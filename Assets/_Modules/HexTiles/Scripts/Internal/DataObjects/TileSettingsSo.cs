using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.HexTiles.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _tileSettings", menuName = "Modules/HexTiles/Settings")]
	public class TileSettingsSo : ScriptableObject
	{
		public TileSettingsVo Vo => settings;
		[HideLabel, SerializeField] TileSettingsVo settings;

		public List<TileMeshPresetSo> Tiles => presets;
		[InlineEditor, SerializeField] List<TileMeshPresetSo> presets = new();
	}
}