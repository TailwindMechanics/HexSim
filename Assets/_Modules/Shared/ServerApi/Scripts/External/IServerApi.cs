using System.Threading.Tasks;
using System;

using Modules.Shared.GameStateRepo.External.Schema;


namespace Modules.Shared.ServerApi.External
{
	public interface IServerApi
	{
		IObservable<(GameState state, double delta)> SeverTickUpdate { get; }
		Task<bool> ServerStartGame (GameState initialState);
		IObservable<GameState> ServerTickStart { get; }
	}
}