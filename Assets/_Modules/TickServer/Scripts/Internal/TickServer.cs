using Unity.Plastic.Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Gameplay.External.DataObjects;
using Modules.TickServer.External;


namespace Modules.TickServer.Internal
{
	public class TickServer : MonoInstaller, ITickServer
	{
		public IObservable<(GameState, double)> TickUpdate => tickUpdate;
		public IObservable<GameState> TickStart => tickStart;

		readonly Subject<(GameState, double)> tickUpdate = new();
		readonly Subject<GameState> tickStart = new();
		const float maxTickRateMs = 10000f;
		const float minTickRateMs = 10f;
		GameState gameState;

		[SerializeField] bool logTicks;

		[Range(minTickRateMs, maxTickRateMs), DisableInPlayMode, SerializeField]
		double tickRateMs = 1000;

		[SerializeField] GameSettingsSo gameSettings;


		public override void InstallBindings()
			=> Container.Bind<ITickServer>().FromInstance(this).AsSingle();

		public override void Start()
			=> Begin();

		public void Begin()
		{
			gameState = new GameState();
			gameSettings.Vo.Teams.ForEach(team =>
			{
				var newUser = new User(team.OwnerUsername, new Team(team));
				newUser.Team.SetOwner(newUser);
				gameState.AddUser(newUser);
			});

			tickStart.OnNext(gameState);
			LogState(gameState, "cyan");

			Observable.Interval(TimeSpan.FromMilliseconds(tickRateMs))
				.Where(_ => tickRateMs is >= minTickRateMs and <= maxTickRateMs)
				.TakeUntilDestroy(this)
				.Timestamp()
				.Pairwise()
				.Select(pair => pair.Current.Timestamp
					.Subtract(pair.Previous.Timestamp)
					.TotalMilliseconds)
				.Select(delta => (gameState, delta))
				.Subscribe(tuple =>
				{
					LogState(tuple.gameState);
					tickUpdate.OnNext(tuple);
				});
		}

		public void MoveActor(User owner, Actor actor, Vector2Int newCoords)
		{
			if (!gameState.UserIsInAnyTeam(owner)) return;
			if (!actor.IsOwnedByUser(owner)) return;

			var localActor = gameState.GetActor(owner, actor.Id);
			if (localActor == null) return;
			if (localActor.IsDead) return;

			gameState.SetActorCoords(owner, actor.Id, newCoords);
		}

		void LogState(GameState state, string color = "yellow")
		{
			if (!logTicks) return;
			Debug.Log($"<color={color}><b>>>> {JsonConvert.SerializeObject(state)}</b></color>");
		}
	}
}