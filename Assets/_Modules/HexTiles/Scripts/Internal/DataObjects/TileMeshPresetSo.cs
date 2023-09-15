using System.Collections.Generic;
using UnityEngine;


namespace Modules.HexTiles.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _tileMesh", menuName = "Modules/HexTiles/Tile Preset")]
	public class TileMeshPresetSo : ScriptableObject
	{
		public List<EdgeLoop> Preset => loops;
		[SerializeField] List<EdgeLoop> loops;
	}
}