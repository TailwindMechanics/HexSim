using Unity.Plastic.Newtonsoft.Json;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Server.TickServer.External;
using Modules.Shared.ServerApi.External;


namespace Modules.Server.TickServer.Internal
{
	public class TickServer : MonoInstaller, ITickServer, IServerApi
	{
		// IServerApi
		public IObservable<(GameState state, double delta)> SeverTickUpdate
			=> TickUpdate;
		public IObservable<GameState> ServerTickStart
			=> TickStart;
		public Task<bool> ServerStartGame(GameState initialState)
			=> Starting(initialState);
		// IServerApi


		public IObservable<GameState> TickStart => tickStart;
		public IObservable<(GameState state, double delta)> TickUpdate => tickUpdate;

		readonly Subject<(GameState, double)> tickUpdate = new();
		readonly Subject<GameState> tickStart = new();
		const float maxTickRateMs = 10000f;
		const float minTickRateMs = 10f;
		GameState gameState;

		[SerializeField] bool logTicks;

		[Range(minTickRateMs, maxTickRateMs), DisableInPlayMode, SerializeField]
		double tickRateMs = 1000;


		public override void InstallBindings()
		{
			Container.Bind<ITickServer>().FromInstance(this).AsSingle();
			Container.Bind<IServerApi>().FromInstance(this).AsSingle();
		}

		async Task<bool> Starting(GameState initialState)
		{
			gameState = initialState;

			await Task.Delay(TimeSpan.FromSeconds(.1));

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

			return true;
		}

		public void MoveActor(User owner, Actor actor, Vector2Int newCoords)
		{
			if (!gameState.UserIsInAnyTeam(owner)) return;
			if (!actor.IsOwnedByUser(owner)) return;

			var localActor = gameState.GetActor(owner, actor.Id);
			if (localActor == null) return;
			if (localActor.IsDead) return;


		}

		void LogState(GameState state, string color = "yellow")
		{
			if (!logTicks) return;
			Debug.Log($"<color={color}><b>>>> {JsonConvert.SerializeObject(state)}</b></color>");
		}
	}
}