using System.Collections.Generic;

using Modules.Shared.GameStateRepo.External.Schema;


namespace Modules.Server.GameLogic.External
{
	public interface IGameLogic
	{
		GameState Init(List<User> users, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY);
		GameState Next(GameState state);
	}
}