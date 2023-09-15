﻿using System.Collections.Generic;
using UnityEngine;

namespace Modules.HexMap.External.DataObjects
{
	public class HexGrid
	{
		public List<Hex2> Cells { get; } = new();
		public List<Hex2> OuterCells { get; } = new();

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

			Debug.Log($"<color=yellow><b>>>> Cells.Count: {Cells.Count}</b></color>");
		}

		void AddCell(int ne, int se, int radius)
		{
			var newCell = new Hex2(ne, se);
			if (!IsWithinHexRadius(newCell, radius)) return;
			if (Cells.Contains(newCell)) return;

			if (!IsOuterCell(newCell, radius)) Cells.Add(newCell);
			else OuterCells.Add(newCell);
		}

		bool IsWithinHexRadius(Hex2 cell, int radius)
			=> Mathf.Sign(cell.ne * cell.se) >= 0
			   || Mathf.Abs(cell.ne) + Mathf.Abs(cell.se) <= radius;

		bool IsOuterCell(Hex2 cell, int radius)
			=> !IsWithinHexRadius(cell, radius-1)
			   || Mathf.Abs((int)cell.ne) == radius
			   || Mathf.Abs((int)cell.se) == radius;
	}
}