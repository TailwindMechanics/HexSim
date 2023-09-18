using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


namespace Modules.Client.HexTiles.External.Schema
{
	[Serializable]
	public class HeightColorMapVo
	{
		public Color? GetColorForHeight(float height)
			=> heightColorRanges
				.Select(heightColorRange => heightColorRange
					.GetColorInRange(height))
				.FirstOrDefault(hexColor => hexColor != null);
		public Color GetLowest ()
			=> heightColorRanges.OrderBy(item => item.MinHeight).First().Color;
		public Color GetHighest ()
			=> heightColorRanges.OrderBy(item => item.MaxHeight).Last().Color;

		[SerializeField] List<HeightColorRange> heightColorRanges;
	}
}