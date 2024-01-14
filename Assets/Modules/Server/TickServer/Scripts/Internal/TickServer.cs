using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;
using Modules.Shared.ServerApi.External;
using Modules.Server.GameLogic.External;
using Modules.Shared.Neese.HexTwo.External.Schema;


namespace Modules.Server.TickServer.Internal
{
	public class TickServer : MonoInstaller, IServerApi
	{
		// IServerApi
		readonly ISubject<Hex2> playerPos = new Subject<Hex2>();
		public void SetPlayerPos(Hex2 newPos)
			=> playerPos.OnNext(newPos);
		public IObservable<GameState> ServerTickStart
			=> tickStart;
		public IObservable<(GameState state, double delta)> SeverTickUpdate
			=> tickUpdate;
		public IObservable<Team> ServerTickEnd
			=> tickEnd;
		public Task<bool> ServerStartGame(GameSettingsVo settings)
			=> Starting(settings);
		// IServerApi


		[Inject] IGameLogic gameLogic;


		readonly Subject<(GameState, double)> tickUpdate = new();
		readonly Subject<GameState> tickStart = new();
		readonly Subject<Team> tickEnd = new();
		const float maxTickRateMs = 10000f;
		const float minTickRateMs = 10f;

		[Range(minTickRateMs, maxTickRateMs), SerializeField]
		double tickRateMs = 1000;

		[Header("Debug")]
		[SerializeField] bool logTicks;
		[SerializeField] int maxTicks;


		public override void InstallBindings()
			=> Container.Bind<IServerApi>().FromInstance(this).AsSingle();

		async Task<bool> Starting(GameSettingsVo settings)
		{
			var users = new List<User>();
			settings.Teams.ForEach(team =>
			{
				var actors = new List<Actor>();
				team.Actors.ForEach(actor =>
				{
					actors.Add(new Actor(actor.ActorPrefab.PrefabId, actor.Hex2Coords, actor.HitPoints));
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
				.Timestamp()
				.Pairwise()
				.Select(pair => pair.Current.Timestamp
					.Subtract(pair.Previous.Timestamp)
					.TotalMilliseconds)
				.Select(delta => (state, delta))
				.TakeUntil(tickEnd)
				.TakeUntilDestroy(this)
				.Subscribe(tuple =>
				{
					LogState(tuple.state);
					var newState = gameLogic.Next(tuple.state);
					if (newState.GetWinner() == null)
					{
						tickUpdate.OnNext((newState, tuple.delta));
					}
					else tickEnd.OnNext(newState.GetWinner().Team);
				});

			playerPos
				.DistinctUntilChanged()
				.Where(pos => Hex2.WithinRadius(pos, state.Radius))
				.TakeUntilDestroy(this)
				.Subscribe(gameLogic.SetPlayerPos);

			if (maxTicks > 0)
			{
				tickUpdate
					.TakeUntilDestroy(this)
					.TakeUntil(tickEnd)
					.Subscribe(_ =>
					{
						maxTicks--;

						if (maxTicks <= 0)
						{
							tickEnd.OnNext(null);
						}
					});
			}

			return true;
		}

		void LogState(GameState state, string color = "yellow")
		{
			if (!logTicks) return;
			Debug.Log($"<color={color}><b>>>> {JsonConvert.SerializeObject(state)}</b></color>");
		}
	}
}