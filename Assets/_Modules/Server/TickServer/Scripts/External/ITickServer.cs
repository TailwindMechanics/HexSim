using System;

using Modules.Shared.GameStateRepo.External.Schema;


namespace Modules.Server.TickServer.External
{
	public interface ITickServer
	{
		IObservable<(GameState state, double delta)> TickUpdate { get; }
		IObservable<GameState> TickStart { get; }
	}
}