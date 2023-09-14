using System.Collections.Generic;


namespace Modules.HexMap.External.DataObjects
{
	public class HexGrid
	{
		public Hex2[,] Cells { get; private set; }
		public List<Hex2> FlatCells => flatCells ??= FlattenCells();

		List<Hex2> flatCells;


		public HexGrid (int gridRadius)
		{
			Cells = new Hex2[gridRadius+1, gridRadius+1];
			for (var x = 0; x < gridRadius; x++)
			{
				for (var y = 0; y < gridRadius; y++)
				{
					Cells[x, y] = new Hex2(x, y);
				}
			}
		}

		List<Hex2> FlattenCells()
		{
			var flatList = new List<Hex2>();
			for (var i = 0; i < Cells.GetLength(0); i++)
			{
				for (var j = 0; j < Cells.GetLength(1); j++)
				{
					flatList.Add(Cells[i, j]);
				}
			}
			return flatList;
		}
	}
}