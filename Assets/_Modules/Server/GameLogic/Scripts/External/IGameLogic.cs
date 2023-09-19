using System.Collections.Generic;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Shared.HexMap.External.Schema;


namespace Modules.Server.GameLogic.External
{
	public interface IGameLogic
	{
		void SetPlayerPos (Hex2 newPos);
		GameState Init(List<User> users, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY);
		GameState Next(GameState state);
	}
}