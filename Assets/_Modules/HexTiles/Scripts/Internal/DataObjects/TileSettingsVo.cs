using System.Collections.Generic;
using UnityEngine;
using System;


namespace Modules.HexTiles.Internal.DataObjects
{
	[Serializable]
	public class TileSettingsVo
	{
		[SerializeField] List<TilePrefabVo> tiles = new();
	}
}