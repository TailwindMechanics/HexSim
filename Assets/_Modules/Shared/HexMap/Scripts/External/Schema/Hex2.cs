using UnityEngine;
using System;


namespace Modules.Shared.HexMap.External.Schema
{
	[Serializable]
	public struct Hex2
	{
		public static bool OutOfBounds(Hex2 cell, int radius)
			=> Mathf.Abs(cell.ne) > radius || Mathf.Abs(cell.se) > radius;
		public static bool ValidHex(Hex2 cell, int radius)
			=> !(Math.Sign(cell.ne) != Math.Sign(cell.se)
			     && Mathf.Abs(cell.ne) + Mathf.Abs(cell.se) > radius);
		public static Hex2 Zero => new(0, 0);
		public static float Distance(Hex2 coord1, Hex2 coord2)
			=> Mathf.Sqrt(Mathf.Pow(coord1.ne - coord2.ne, 2) + Mathf.Pow(coord1.se - coord2.se, 2));
		public static Vector3 NeAxis
			=> -new Vector3(Mathf.Cos(Mathf.PI / 3), 0, Mathf.Sin(Mathf.PI / 3));
		public static Vector3 SeAxis
			=> -new Vector3(Mathf.Cos(-Mathf.PI / 3), 0, Mathf.Sin(-Mathf.PI / 3));

		public float ne;
		public float se;
		public float e => (ne + se) / 2;

		public Hex2 (Vector2Int coords)
		{
			ne = coords.x;
			se = coords.y;
		}

		public Hex2 (double ne, double se)
		{
			this.ne = (float)ne;
			this.se = (float)se;
		}

		public Hex2 (float ne, float se)
		{
			this.ne = ne;
			this.se = se;
		}

		public override string ToString()
			=> $"({ne}, {se})";
		public static bool operator ==(Hex2 a, Hex2 b)
			=> Math.Abs(a.ne - b.ne) < .001f && Math.Abs(a.se - b.se) < .001f;
		public static bool operator !=(Hex2 a, Hex2 b)
			=> !(a == b);
	}
}