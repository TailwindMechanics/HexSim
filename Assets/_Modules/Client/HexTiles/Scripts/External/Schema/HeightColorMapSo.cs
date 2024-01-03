using UnityEngine;


namespace Modules.Client.HexTiles.External.Schema
{
	[CreateAssetMenu(fileName = "new _heightColorMap", menuName = "Modules/HexTiles/Height Color Map")]
	public class HeightColorMapSo : ScriptableObject
	{
		public HeightColorMapVo Vo => heightColorMap;
		[SerializeField] HeightColorMapVo heightColorMap;
	}
}