using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.MouseInput.External.Schema;
using Modules.Client.SpawnerService.External;
using Modules.Shared.HexMap.External.Schema;
using Modules.Client.Actors.External.Schema;
using Modules.Client.MouseInput.External;
using Modules.Shared.ServerApi.External;
using Modules.Client.Utilities.External;


namespace Modules.Client.Actors.Internal
{
	public class Actors : MonoBehaviour
	{
		[Inject] ISpawnerService spawnerService;
		[Inject] IMouseInput mouseInput;
		[Inject] IServerApi server;

		[SerializeField] Transform opponentActorsContainer;
		[SerializeField] Transform playerActorsContainer;
		[SerializeField] Transform clickMarker;
		[InlineEditor, SerializeField] ActorPrefabMappingSo prefabMap;

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

					var playerTeam = SpawnTeamActors(state.Users[0].Team, state, prefabMap, playerActorsContainer);
					var opponentTeam = SpawnTeamActors(state.Users[1].Team, state, prefabMap, opponentActorsContainer);
				});

			mouseInput.LmbViewportState
				.WithLatestFrom(server.SeverTickUpdate, (tuple, serverTuple)
					=> (mouseState: tuple.state, mousePos: tuple.pos, gameState: serverTuple.state))
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.mouseState == MouseState.Click)
				.Select(tuple => (tuple.mousePos, tuple.gameState))
				.Subscribe(tuple => OnMouseClick(tuple.mousePos, tuple.gameState, tuple.gameState.Amplitude / 2f));
		}

		void OnMouseClick(Vector3 clickScreenPos, GameState gameState, float height)
		{
			var worldPos = cam.ScreenToPlane(clickScreenPos, xzPlane, height);
			if (worldPos == null) return;

			var hexCoords = worldPos.Value.ToHex2().BankersRound();
			Debug.Log($"<color=yellow><b>>>> coords: {hexCoords}</b></color>");
			var clickedCoords = hexCoords.ToVector3();
			var seed = gameState.Seed.ToSeedFloat();
			var offset = new Vector2Int(gameState.NoiseOffsetX, gameState.NoiseOffsetY);
			clickedCoords.y = hexCoords.PerlinHeight(seed, gameState.NoiseScale, gameState.Amplitude, offset);

			clickMarker.position = clickedCoords;
		}

		IEnumerable<GameObject> SpawnTeamActors (Team team, GameState state, ActorPrefabMappingSo map, Transform parent)
		{
			var seed = state.Seed.ToSeedFloat();
			var scale = state.NoiseScale;
			var amp = state.Amplitude;
			var offset = new Vector2Int(state.NoiseOffsetX, state.NoiseOffsetY);

			var result = new List<GameObject>();
			team.Actors.ForEach(actor =>
			{
				var spawnPos = actor.Coords.ToVector3();
				var height = actor.Coords.PerlinHeight(seed, scale, amp, offset);
				spawnPos.y = height;

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