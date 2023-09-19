using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

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
		}

		void OnStarted () => Debug.Log("<color=green><b>>>> GameSetup::OnStarted</b></color>");
		void OnFailedToStart () => Debug.LogError("GameSetup::OnFailedToStart");
	}
}