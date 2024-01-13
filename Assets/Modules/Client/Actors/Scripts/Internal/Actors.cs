using System.Collections.Generic;
using System.Linq;
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
		[SerializeField] LineRenderer pathsPrefab;
		[SerializeField] Transform clickMarker;
		[SerializeField] Transform pathsParent;
		[SerializeField] float lerpSmoothing = 0.1f;
		[SerializeField] ActorPrefabMappingSo prefabMap;

		static readonly int baseColor = Shader.PropertyToID("_BaseColor");
		readonly Plane xzPlane = new(Vector3.up, Vector3.zero);
		Camera cam;


		void Awake()
			=> cam = Camera.main;

		void Start()
		{
			var spawnedActors= new Dictionary<Guid, (Transform spawned, List<Renderer> renderers)>();
			var actorHealths = new Dictionary<Guid, int>();

			server.ServerTickStart
				.Delay(TimeSpan.FromSeconds(.3f))
				.TakeUntilDestroy(this)
				.Subscribe(state =>
				{
					if (state.Users.Count < 1)
					{
						Debug.LogError("You must set at least one team in game settings.");
						return;
					}

					spawnedActors = SpawnTeamActors(state.Users[0].Team, state, prefabMap, playerActorsContainer);

					if (state.Users.Count > 1)
					{
						var opponent = SpawnTeamActors(state.Users[1].Team, state, prefabMap, opponentActorsContainer);
						foreach (var item in opponent) spawnedActors.Add(item.Key, item.Value);
					}
				});

			server.ServerTickEnd
				.TakeUntilDestroy(this)
				.Where(winner => winner != null)
				.SelectMany(winner => winner.Actors)
				.Subscribe(actor =>
				{
					actorHealths.TryAdd(actor.Id, actor.Health);
					SetHitColour(actor.Id, false, spawnedActors);
					actorHealths[actor.Id] = actor.Health;
				});

			var serverTickUpdateStream = server.SeverTickUpdate
				.TakeUntilDestroy(this)
				.Share();

			Observable.EveryLateUpdate()
				.WithLatestFrom(serverTickUpdateStream, (_, serverTuple) => serverTuple)
				.SelectMany(tuple => tuple.state.Users.Select(user => (user, tuple.state)))
				.SelectMany(tuple => tuple.user.Team.Actors.Select(actor => (actor, tuple.state)))
				.TakeUntilDestroy(this)
				.Subscribe(data =>
				{
					var (actor, state) = data;

					if (actor.IsDead)
					{
						spawnedActors[actor.Id].spawned.localEulerAngles = new Vector3(0, 0, 90);
					}
					else
					{
						var newPos = actor.Coords.ToVector3();
						newPos.y = HeightAtCoords(actor.Coords, state);

						var currentPos = spawnedActors[actor.Id].spawned.position;
						spawnedActors[actor.Id].spawned.position = Vector3.Lerp(currentPos, newPos, lerpSmoothing * Time.deltaTime);
					}
				});

			serverTickUpdateStream
				.TakeUntilDestroy(this)
				.Subscribe(tuple =>
				{
					pathsParent.DestroyAllChildren();

					var state = tuple.state;
					foreach (var actor in state.Users.SelectMany(user => user.Team.Actors))
					{
						actorHealths.TryAdd(actor.Id, actor.Health);
						SetHitColour(actor.Id, !actor.IsDead && actor.Health < actorHealths[actor.Id], spawnedActors);
						actorHealths[actor.Id] = actor.Health;

						if (actor.IsDead || actor.NavPath is not { Count: > 0 }) continue;

						var line = Instantiate(pathsPrefab, pathsParent, true);
						line.name = $"Path_{actor.Coords}";
						line.positionCount = actor.NavPath.Count;
						for (var index = 0; index < actor.NavPath.Count; index++)
						{
							var pos = actor.NavPath[index];
							pos.y = HeightAtCoords(pos.ToHex2(), state) + 0.2f;
							line.SetPosition(index, pos);
						}
					}
				});

			mouseInput.LmbViewportState
				.WithLatestFrom(server.SeverTickUpdate, (tuple, serverTuple)
					=> (mouseState: tuple.state, mousePos: tuple.pos, gameState: serverTuple.state))
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.mouseState == MouseState.Click)
				.Select(tuple => (tuple.mousePos, tuple.gameState))
				.Subscribe(tuple => OnMouseClick(tuple.mousePos, tuple.gameState, tuple.gameState.Amplitude / 2f));
		}

		float HeightAtCoords(Hex2 coords, GameState state)
			=> coords.PerlinHeight(state.SeedAsFloat, state.NoiseScale, state.Amplitude, state.NoiseOffsetX, state.NoiseOffsetY);

		void OnMouseClick(Vector3 clickScreenPos, GameState gameState, float height)
		{
			var worldPos = cam.ScreenToPlane(clickScreenPos, xzPlane, height);
			if (worldPos == null) return;

			var targetPos = SetMarkerPosition(worldPos.Value.ToHex2(), gameState);
			server.SetPlayerPos(targetPos.ToHex2());
			clickMarker.position = targetPos;
		}

		void SetHitColour (Guid id, bool gotHit, Dictionary<Guid, (Transform spawned, List<Renderer> renderers)> spawnedActors)
		{
			var propertyBlock = new MaterialPropertyBlock();
			var color = gotHit ? Color.red : Color.white;
			var tuple = spawnedActors[id];
			propertyBlock.SetColor(baseColor, color);
			tuple.renderers.ForEach(rend => rend.SetPropertyBlock(propertyBlock));
		}

		Vector3 SetMarkerPosition (Hex2 coords, GameState gameState)
		{
			var hexCoords = coords.BankersRound();
			var clickedCoords = hexCoords.ToVector3();
			Debug.Log($"<color=yellow><b>>>> {hexCoords}</b></color>");
			var seed = gameState.SeedAsFloat;
			var offset = new Vector2Int(gameState.NoiseOffsetX, gameState.NoiseOffsetY);
			clickedCoords.y = hexCoords.PerlinHeight(seed, gameState.NoiseScale, gameState.Amplitude, offset.x, offset.y);

			return clickedCoords;
		}

		Dictionary<Guid, (Transform, List<Renderer>)> SpawnTeamActors (Team team, GameState state, ActorPrefabMappingSo map, Transform parent)
		{
			var seed = state.SeedAsFloat;
			var scale = state.NoiseScale;
			var amp = state.Amplitude;
			var offset = new Vector2Int(state.NoiseOffsetX, state.NoiseOffsetY);
			var result = new Dictionary<Guid, (Transform, List<Renderer>)>();

			team.Actors.ForEach(actor =>
			{
				var spawnPos = actor.Coords.ToVector3();
				var height = actor.Coords.PerlinHeight(seed, scale, amp, offset.x, offset.y);
				spawnPos.y = height;

				var prefab = map.GetPrefabById(actor.ActorPrefabId);
				if (prefab == null)
				{
					Debug.LogError($"<color=red><b>>>> Could not find prefab for actor: {actor.ActorPrefabId}</b></color>");
					return;
				}

				var spawnedName = $"{team.TeamName}_{actor.ActorPrefabId}_{actor.Id}";
				var spawned = spawnerService.Spawn(prefab, parent, spawnPos, spawnedName);
				var renderers = spawned.GetComponentsInChildren<Renderer>().ToList();
				result.Add(actor.Id, (spawned.transform, renderers));
			});

			return result;
		}
	}
}