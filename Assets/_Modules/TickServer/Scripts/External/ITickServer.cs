using UnityEngine;
using System;

using Modules.Gameplay.External.DataObjects;


namespace Modules.TickServer.External
{
	public interface ITickServer
	{
		public void MoveActor (User owner, Actor actor, Vector2Int newCoords);
		public IObservable<(GameState state, GameSettingsVo settings)> TickStart { get; }
		public IObservable<(GameState state, double delta)> TickUpdate { get; }
	}
}