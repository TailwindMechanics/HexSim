using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.HexTiles.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _tileSettings", menuName = "Modules/HexTiles/Settings")]
	public class TileSettingsSo : ScriptableObject
	{
		public TileSettingsVo Vo => settings;
		[HideLabel, SerializeField] TileSettingsVo settings;
	}
}