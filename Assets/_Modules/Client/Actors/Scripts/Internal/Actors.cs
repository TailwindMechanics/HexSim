using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.MouseInput.External.Schema;
using Modules.Client.SpawnerService.External;
using Modules.Client.Actors.External.Schema;
using Modules.Client.MouseInput.External;
using Modules.Shared.ServerApi.External;
using Modules.Client.Utilities.External;
using Modules.Shared.HexMap.External;


namespace Modules.Client.Actors.Internal
{
	public class Actors : MonoBehaviour
	{
		[Inject] ISpawnerService spawnerService;
		[Inject] IMouseInput mouseInput;
		[Inject] IServerApi server;

		[Range(0f, 1f), SerializeField] float clickPlaneHeight = 0.3f;
		[SerializeField] Transform opponentActorsContainer;
		[SerializeField] Transform playerActorsContainer;
		[SerializeField] ActorPrefabMappingSo prefabMap;
		[SerializeField] Transform clickMarker;

		readonly Plane xzPlane = new(Vector3.up, Vector3.zero);
		Camera cam;


		void Awake()
			=> cam = Camera.main;

		void Start()
		{
			server.ServerTickStart
				.Delay(TimeSpan.FromSeconds(.3f))
				.TakeUntilDestroy(this)
				.Subscribe(state =>
				{
					if (state.Users.Count < 2)
					{
						Debug.LogError($"You must set at least two teams in game settings.");
						return;
					}

					var playerTeam = SpawnTeamActors(state.Users[0].Team, clickPlaneHeight, prefabMap, playerActorsContainer);
					var opponentTeam = SpawnTeamActors(state.Users[1].Team, clickPlaneHeight, prefabMap, opponentActorsContainer);
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
		}

		IEnumerable<GameObject> SpawnTeamActors (Team team, float spawnHeight, ActorPrefabMappingSo map, Transform parent)
		{
			var result = new List<GameObject>();
			team.Actors.ForEach(actor =>
			{
				var spawnPos = actor.Coords.ToVector3();
				spawnPos.y = spawnHeight;

				var prefab = map.GetPrefabById(actor.PrefabId);
				if (prefab == null)
				{
					Debug.LogError($"<color=red><b>>>> Could not find prefab for actor: {actor.PrefabId}</b></color>");
					return;
				}

				var spawnedName = $"{team.TeamName}_{actor.PrefabId}_{actor.Id}";
				var spawned = spawnerService.Spawn(prefab, parent, spawnPos, spawnedName);
				result.Add(spawned);
			});

			return result;
		}
	}
}