using UnityEngine;
using System;

using Modules.Gameplay.External.DataObjects;


namespace Modules.TickServer.External
{
	public interface ITickServer
	{
		public void MoveActor (User owner, Actor actor, Vector2Int newCoords);
		public IObservable<(GameState, double)> TickUpdate { get; }
		public IObservable<GameState> TickStart { get; }
	}
}