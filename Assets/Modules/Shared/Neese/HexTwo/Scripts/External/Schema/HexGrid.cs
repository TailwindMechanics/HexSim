using System.Collections.Generic;

namespace Modules.Shared.Neese.HexTwo.External.Schema
{
	public class HexGrid
	{
		public List<Hex2> Cells { get; } = new();

		public HexGrid(int gridRadius)
		{
			for (var ne = 0; ne <= gridRadius; ne++)
			{
				for (var se = 0; se <= gridRadius; se++)
				{
					AddCell(ne, se, gridRadius);
					AddCell(-ne, -se, gridRadius);
					AddCell(-ne, se, gridRadius);
					AddCell(ne, -se, gridRadius);
				}
			}
		}

		void AddCell(int ne, int se, int radius)
		{
			var newCell = new Hex2(ne, se);
			if (!Hex2.WithinRadius(newCell, radius)) return;
			if (Cells.Contains(newCell)) return;

			Cells.Add(newCell);
		}
	}
}