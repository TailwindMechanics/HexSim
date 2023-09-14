
using UnityEngine;


namespace Modules.HexMap.External.DataObjects
{
	public struct Hex2
	{
		public static Vector3 NeAxis
			=> -new Vector3(Mathf.Cos(Mathf.PI / 3), 0, Mathf.Sin(Mathf.PI / 3));
		public static Vector3 SeAxis
			=> -new Vector3(Mathf.Cos(-Mathf.PI / 3), 0, Mathf.Sin(-Mathf.PI / 3));

		public float ne { get; set; }
		public float se { get; set; }
		public float e => (ne + se) / 2;

		public Hex2 (float ne, float se)
		{
			this.ne = ne;
			this.se = se;
		}

		public override string ToString()
			=> $"({ne}, {se})";
	}
}