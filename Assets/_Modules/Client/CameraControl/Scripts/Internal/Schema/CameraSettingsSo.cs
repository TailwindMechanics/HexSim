using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.Client.CameraControl.Internal.Schema
{
	[CreateAssetMenu(fileName = "new _cameraSettings", menuName = "Modules/CameraControl/Settings")]
	public class CameraSettingsSo : ScriptableObject
	{
		public CameraSettingsVo Vo => settings;
		[HideLabel, SerializeField] CameraSettingsVo settings = new();
	}
}