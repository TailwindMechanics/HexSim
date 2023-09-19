using System.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;


namespace Modules.Shared.ServerApi.External
{
	public interface IServerApi
	{
		ISubject<(User owner, Actor actor, Vector2Int newCoords)> MoveActor { get; }
		IObservable<(GameState state, double delta)> SeverTickUpdate { get; }
		Task<bool> ServerStartGame (GameSettingsVo settings);
		IObservable<GameState> ServerTickStart { get; }
	}
}