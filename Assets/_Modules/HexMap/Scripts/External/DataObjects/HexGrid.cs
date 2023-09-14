using System.Collections.Generic;
using UnityEngine;


namespace Modules.HexMap.External.DataObjects
{
	public class HexGrid
	{
		public List<Hex2> Cells { get; } = new();

		public HexGrid (int gridRadius)
		{
			Debug.Log(-gridRadius);
			for (var x = 0; x <= gridRadius; x++)
			{
				for (var y = 0; y <= gridRadius; y++)
				{
					if (x * y > -gridRadius) Cells.Add(new Hex2(x, y));
					if (-x * y > -gridRadius) Cells.Add(new Hex2(-x, y));
					if (-x * -y > -gridRadius) Cells.Add(new Hex2(-x, -y));
					if (x * -y > -gridRadius) Cells.Add(new Hex2(x, -y));
				}
			}
		}
	}
}