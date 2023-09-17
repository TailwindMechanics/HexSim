using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.Client.GameSetup.External.Schema
{
	[CreateAssetMenu(fileName = "new _gameSettings", menuName = "Modules/Gameplay/Settings")]
	public class GameSettingsSo : ScriptableObject
	{
		public GameSettingsVo Vo => settings;
		[HideLabel, SerializeField] GameSettingsVo settings = new();
	}
}