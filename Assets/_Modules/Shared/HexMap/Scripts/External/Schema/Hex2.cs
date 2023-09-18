using UnityEngine;
using System;


namespace Modules.Shared.HexMap.External.Schema
{
	[Serializable]
	public struct Hex2
	{
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
	}
}