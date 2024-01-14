using System.Threading.Tasks;
using System;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;
using Modules.Shared.Neese.HexTwo.External.Schema;


namespace Modules.Shared.ServerApi.External
{
	public interface IServerApi
	{
		IObservable<Team> ServerTickEnd { get; }
		public void SetPlayerPos (Hex2 newPos);
		IObservable<(GameState state, double delta)> SeverTickUpdate { get; }
		Task<bool> ServerStartGame (GameSettingsVo settings);
		IObservable<GameState> ServerTickStart { get; }
	}
}