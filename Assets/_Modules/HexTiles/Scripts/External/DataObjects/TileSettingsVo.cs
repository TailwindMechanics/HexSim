using System.Collections.Generic;
using UnityEngine;
using System;


namespace Modules.HexTiles.External.DataObjects
{
	[Serializable]
	public class TileSettingsVo
	{
		public List<TilePrefabVo> Tiles => tiles;

		[SerializeField] List<TilePrefabVo> tiles = new();
	}
}