using System.Collections.Generic;
using UnityEngine;


namespace Modules.HexTiles.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _tileMesh", menuName = "Modules/HexTiles/Tile Preset")]
	public class TileMeshPresetSo : ScriptableObject
	{
		public float MinHeight => minHeight;
		public float MaxHeight => maxHeight;
		public List<EdgeLoop> EdgeLoops => loops;
		public bool IsFlatTop => isFlatTop;

		[SerializeField] float minHeight;
		[SerializeField] float maxHeight = .5f;
		[SerializeField] bool isFlatTop;
		[SerializeField] List<EdgeLoop> loops;
	}
}