using System.Collections.Generic;
using UnityEngine;
using System;


namespace Modules.Shared.HexMap.External.Schema
{
	[Serializable]
	public struct Hex2
	{
		public bool Equals(Hex2 other)
			=> ne.Equals(other.ne) && se.Equals(other.se);
		public override bool Equals(object obj)
			=> obj is Hex2 other && Equals(other);
		public override int GetHashCode()
			=> HashCode.Combine(ne, se);
		public static float Dot(Hex2 a, Hex2 b)
			=> a.ne * b.ne + a.se * b.se;

		public static List<Hex2> GetNeighbors(Hex2 currentCoords)
		{
			var ne = currentCoords.ne;
			var se = currentCoords.se;

			return new List<Hex2>
			{
				new (ne + 1, se + 1),
				new (ne - 1, se - 1),
				new (ne + 1, se),
				new (ne - 1, se),
				new (ne, se + 1),
				new (ne, se - 1),
			};
		}

		public static List<Vector3> GetWorldNeighbors(Vector3 worldPos)
		{
			var currentCoords = worldPos.ToHex2();
			var neighbors = GetNeighbors(currentCoords);
			var result = new List<Vector3>();
			foreach (var neighbor in neighbors)
			{
				result.Add(neighbor.ToVector3());
			}
			return result;
		}

		public static Hex2 Random(int radius)
		{
			var ne = UnityEngine.Random.Range(-radius, radius + 1);
			var se = UnityEngine.Random.Range(-radius, radius + 1);
			return new Hex2(ne, se);
		}

		public static bool WithinRadius (Hex2 coords, int radius)
		{
			if (!ValidHexGridLayout(coords, radius)) return false;

			for (var ne = 0; ne <= radius; ne++)
			{
				for (var se = 0; se <= radius; se++)
				{
					if (coords == new Hex2(ne, se)) return true;
					if (coords == new Hex2(-ne, -se)) return true;
					if (coords == new Hex2(-ne, se)) return true;
					if (coords == new Hex2(ne, -se)) return true;
				}
			}

			return false;
		}

		public static Hex2 Zero => new(0, 0);
		public static float Distance(Hex2 coord1, Hex2 coord2)
			=> Mathf.Abs(Mathf.Sqrt(Mathf.Pow(coord1.ne - coord2.ne, 2) + Mathf.Pow(coord1.se - coord2.se, 2)));
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

		static bool ValidHexGridLayout(Hex2 cell, int radius)
			=> Math.Sign(cell.ne) == Math.Sign(cell.se)
			   || Mathf.Abs(cell.ne) + Mathf.Abs(cell.se) <= radius;
		public override string ToString()
			=> $"({ne}, {se})";
		public static bool operator ==(Hex2 a, Hex2 b)
			=> Math.Abs(a.ne - b.ne) < .001f && Math.Abs(a.se - b.se) < .001f;
		public static bool operator !=(Hex2 a, Hex2 b)
			=> !(a == b);
		public static Hex2 operator +(Hex2 a, Hex2 b)
			=> new(a.ne + b.ne, a.se + b.se);
		public static Hex2 operator -(Hex2 a, Hex2 b)
			=> new(a.ne - b.ne, a.se - b.se);
		public static Hex2 operator *(Hex2 a, float scalar)
			=> new(a.ne * scalar, a.se * scalar);
		public static Hex2 operator /(Hex2 a, float scalar)
		{
			if (Math.Abs(scalar) < .001f) scalar = .001f;
			return new Hex2(a.ne / scalar, a.se / scalar);
		}
	}
}