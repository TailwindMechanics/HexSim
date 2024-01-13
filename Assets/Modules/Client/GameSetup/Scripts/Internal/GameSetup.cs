using UnityEngine;
using Zenject;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.GameSetup.External.Schema;
using Modules.Shared.ServerApi.External;
using UnityEngine.SceneManagement;


namespace Modules.Client.GameSetup.Internal
{
	public class GameSetup : MonoBehaviour
	{
		[Inject] IServerApi serverApi;

		[SerializeField]
		GameSettingsSo gameSettings;


		async void Start ()
		{
			Debug.Log("<color=yellow><b>>>> GameSetup::Start</b></color>");
			var started = await serverApi.ServerStartGame(gameSettings.Vo);
			if (started)OnStarted();
			else OnFailedToStart();

			serverApi.ServerTickEnd
				.Take(1)
				.TakeUntilDestroy(this)
				.Subscribe(OnEnded);

			Observable.EveryUpdate()
				.Where(_ => RestartKeypress())
				.Take(1)
				.TakeUntilDestroy(this)
				.Subscribe(_ =>
				{
					var currentScene = SceneManager.GetActiveScene();
					SceneManager.LoadScene(currentScene.name);
				});
		}

		bool RestartKeypress ()
			=> Input.GetKeyDown(KeyCode.Return)
			   || Input.GetKeyDown(KeyCode.KeypadEnter)
			   || Input.GetKeyDown(KeyCode.Escape)
			   || Input.GetKeyDown(KeyCode.R);

		void OnStarted () => Debug.Log("<color=#00FFFF><b>>>> GameSetup::OnStarted</b></color>");
		void OnFailedToStart () => Debug.LogError("GameSetup::OnFailedToStart");
		void OnEnded (Team winner)
		{
			if (winner == null)
			{
				Debug.Log("<color=orange><b>>>> Game ended, no winner</b></color>");
				return;
			}

			switch (winner.TeamName)
			{
				case "Red":
					Debug.Log("<color=red><b>>>> You Lost :/</b></color>");
					break;
				case "Blue":
					Debug.Log("<color=green><b>>>> You won! :D</b></color>");
					break;
				default:
					Debug.Log("<color=orange><b>>>> It's a draw :|</b></color>");
					break;
			}
		}
	}
}