using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

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
			Debug.Log("<color=yellow><b>>>> GameSetup::Start</b></color>");
			var started = await serverApi.ServerStartGame(gameSettings.Vo);
			if (started)OnStarted();
			else OnFailedToStart();

			serverApi.ServerTickEnd
				.Take(1)
				.TakeUntilDestroy(this)
				.Subscribe(OnEnded);
		}

		void OnStarted () => Debug.Log("<color=cyan><b>>>> GameSetup::OnStarted</b></color>");
		void OnFailedToStart () => Debug.LogError("GameSetup::OnFailedToStart");
		void OnEnded (Team winner) => Debug.Log($"<color=green><b>>>> GameSetup::OnEnded: {winner.TeamName} team won!</b></color>");
	}
}