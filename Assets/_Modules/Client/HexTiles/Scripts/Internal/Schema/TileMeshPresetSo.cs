using System.Collections.Generic;
using UnityEngine;


namespace Modules.Client.HexTiles.Internal.Schema
{
	[CreateAssetMenu(fileName = "new _tileMesh", menuName = "Modules/HexTiles/Tile Preset")]
	public class TileMeshPresetSo : ScriptableObject
	{
		public List<EdgeLoop> EdgeLoops => loops;
		public bool IsFlatTop => isFlatTop;

		[SerializeField] bool isFlatTop;
		[SerializeField] List<EdgeLoop> loops;
	}
}