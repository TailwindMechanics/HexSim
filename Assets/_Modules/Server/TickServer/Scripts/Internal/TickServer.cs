using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;
using Modules.Server.TickServer.External;
using Modules.Shared.ServerApi.External;
using Modules.Server.GameLogic.External;


namespace Modules.Server.TickServer.Internal
{
	public class TickServer : MonoInstaller, ITickServer, IServerApi
	{
		// IServerApi
		public ISubject<(User owner, Actor actor, Vector2Int newCoords)> MoveActor { get; }
			= new Subject<(User owner, Actor actor, Vector2Int newCoords)>();

		public IObservable<(GameState state, double delta)> SeverTickUpdate
			=> TickUpdate;
		public IObservable<GameState> ServerTickStart
			=> TickStart;
		public Task<bool> ServerStartGame(GameSettingsVo settings)
			=> Starting(settings);
		// IServerApi


		[Inject] IGameLogic gameLogic;

		public IObservable<GameState> TickStart => tickStart;
		public IObservable<(GameState state, double delta)> TickUpdate => tickUpdate;

		readonly Subject<(GameState, double)> tickUpdate = new();
		readonly Subject<GameState> tickStart = new();
		const float maxTickRateMs = 10000f;
		const float minTickRateMs = 10f;

		[SerializeField] bool logTicks;

		[Range(minTickRateMs, maxTickRateMs), DisableInPlayMode, SerializeField]
		double tickRateMs = 1000;


		public override void InstallBindings()
		{
			Container.Bind<ITickServer>().FromInstance(this).AsSingle();
			Container.Bind<IServerApi>().FromInstance(this).AsSingle();
		}

		async Task<bool> Starting(GameSettingsVo settings)
		{
			var users = new List<User>();
			settings.Teams.ForEach(team =>
			{
				var actors = new List<Actor>();
				team.Actors.ForEach(actor =>
				{
					actors.Add(new Actor(actor.ActorPrefab.PrefabId, actor.Hex2Coords));
				});
				var newUser = new User(team.OwnerUsername, new Team(team.TeamName, actors));
				newUser.Team.SetOwner(newUser);
				users.Add(newUser);
			});

			var state = gameLogic.Init(
				users,
				settings.GridRadius,
				settings.Seed,
				settings.MinWalkHeight,
				settings.Amplitude,
				settings.NoiseScale,
				settings.NoiseOffset.x,
				settings.NoiseOffset.y
			);

			await Task.Delay(TimeSpan.FromSeconds(.1));

			tickStart.OnNext(state);
			LogState(state, "cyan");

			Observable.Interval(TimeSpan.FromMilliseconds(tickRateMs))
				.Where(_ => tickRateMs is >= minTickRateMs and <= maxTickRateMs)
				.TakeUntilDestroy(this)
				.Timestamp()
				.Pairwise()
				.Select(pair => pair.Current.Timestamp
					.Subtract(pair.Previous.Timestamp)
					.TotalMilliseconds)
				.Select(delta => (state, delta))
				.Subscribe(tuple =>
				{
					LogState(tuple.state);
					var newState = gameLogic.Next(tuple.state);
					tickUpdate.OnNext((newState, tuple.delta));
				});

			MoveActor
				.Where(tuple => state.UserIsInAnyTeam(tuple.owner))
				.Where(tuple => tuple.actor.IsOwnedByUser(tuple.owner))
				.Select(tuple => (tuple.owner, localActor: state.GetActor(tuple.owner, tuple.actor.Id), tuple.newCoords))
				.Where(tuple => tuple.localActor is not null && !tuple.localActor.IsDead)
				.TakeUntilDestroy(this)
				.Subscribe(OnMoveActor);

			return true;
		}

		void OnMoveActor((User owner, Actor actor, Vector2Int newCoords) tuple)
		{

		}

		void LogState(GameState state, string color = "yellow")
		{
			if (!logTicks) return;
			Debug.Log($"<color={color}><b>>>> {JsonConvert.SerializeObject(state)}</b></color>");
		}
	}
}