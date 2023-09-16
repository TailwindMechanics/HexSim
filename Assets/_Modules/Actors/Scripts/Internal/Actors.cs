using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

using Modules.Gameplay.External.DataObjects;
using Modules.TickServer.External;
using Modules.MouseInput.External;
using Modules.Utilities.External;
using Modules.HexMap.External;
using Modules.HexMap.External.DataObjects;
using Modules.SpawnerService.External;


namespace Modules.Actors.Internal
{
	public class Actors : MonoBehaviour
	{
		[Inject] ISpawnerService spawnerService;
		[Inject] IMouseInput mouseInput;
		[Inject] ITickServer tickServer;

		[SerializeField] Transform playerActorsContainer;
		[SerializeField] Transform opponentActorsContainer;
		[SerializeField] Transform clickMarker;
		[Range(0f, 1f), SerializeField] float clickPlaneHeight = 0.3f;

		readonly Plane xzPlane = new(Vector3.up, Vector3.zero);
		Camera cam;

		User opponent;
		User player;

		Dictionary<Actor, GameObject> opponentActors = new();
		Dictionary<Actor, GameObject> playerActors = new();


		void Awake()
			=> cam = Camera.main;

		void Start()
		{
			tickServer.TickStart
				.TakeUntilDestroy(this)
				.Subscribe(tuple =>
				{
					var (state, settings) = tuple;
					if (state.Users.Count < 2)
					{
						Debug.Log($"<color=yellow><b>>>> You must set at least two teams in game settings.</b></color>");
						return;
					}

					var playerSettings = settings.Teams[0];
					player = state.GetUserByTeamName(playerSettings.TeamName);

					var opponentSettings = settings.Teams[1];
					opponent = state.GetUserByTeamName(opponentSettings.TeamName);

					playerActors.Clear();
					playerSettings.ActorCoords.ForEach(coord =>
					{
						var hexCoord = new Hex2(coord).Round();
						var newActor = new Actor((int)hexCoord.ne, (int)hexCoord.se);
						var spawnPos = hexCoord.ToVector3();
						spawnPos.y = clickPlaneHeight;
						var spawned = spawnerService.Spawn(playerSettings.ActorPrefab,
							playerActorsContainer,
							spawnPos,
							$"PlayerActor_{hexCoord.ToString()}");
						playerActors.Add(newActor, spawned);
					});

					opponentActors.Clear();
					opponentSettings.ActorCoords.ForEach(coord =>
					{
						var hexCoord = new Hex2(coord).Round();
						var newActor = new Actor((int)hexCoord.ne, (int)hexCoord.se);
						var spawnPos = hexCoord.ToVector3();
						spawnPos.y = clickPlaneHeight;
						var spawned = spawnerService.Spawn(opponentSettings.ActorPrefab,
							opponentActorsContainer,
							spawnPos,
							$"OpponentActor_{hexCoord.ToString()}");
						opponentActors.Add(newActor, spawned);
					});

					Debug.Log($"<color=yellow><b>>>> state: {state.Users}</b></color>");
				});

			mouseInput.LmbState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state == MouseState.Click)
				.Select(tuple => tuple.pos)
				.Subscribe(OnMouseClick);
		}

		void OnMouseClick(Vector3 clickScreenPos)
		{
			var worldPos = cam.ScreenToPlane(clickScreenPos, xzPlane, clickPlaneHeight);
			if (worldPos == null) return;

			var clickedCoords = worldPos.Value.ToHex2().Round().ToVector3();
			clickedCoords.y = worldPos.Value.y;
			clickMarker.position = clickedCoords;

			// tickServer.MoveActor();
		}
	}
}