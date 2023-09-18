using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;
using Modules.Shared.ServerApi.External;


namespace Modules.Client.GameSetup.Internal
{
	public class GameSetup : MonoBehaviour
	{
		[Inject] IServerApi serverApi;

		[InlineEditor, SerializeField]
		GameSettingsSo gameSettings;


		async void Start ()
		{
			var users = new List<User>();
			gameSettings.Vo.Teams.ForEach(team =>
			{
				var actors = new List<Actor>();
				team.Actors.ForEach(actor =>
				{
					actors.Add(new Actor(actor.ActorPrefab.PrefabId, actor.Hex2Coords));
				});
				var newUser = new User(team.OwnerUsername, new Team(team.TeamName, actors));
				newUser.Team.SetOwner(newUser);
				users.Add(newUser);
			});

			Debug.Log("<color=yellow><b>>>> GameSetup::Start</b></color>");
			var settings = gameSettings.Vo;
			var gameState = new GameState(
				settings.GridRadius,
				settings.Seed,
				settings.HeightColorMap,
				settings.Amplitude,
				settings.NoiseScale,
				settings.NoiseOffset,
				users
			);
			var started = await serverApi.ServerStartGame(gameState);

			if (started)OnStarted();
			else OnFailedToStart();
		}

		void OnStarted () => Debug.Log("<color=green><b>>>> GameSetup::OnStarted</b></color>");
		void OnFailedToStart () => Debug.LogError("GameSetup::OnFailedToStart");
	}
}