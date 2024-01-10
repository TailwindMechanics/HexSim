using UnityEngine;
using System;

namespace Modules.Server.NeuroNavigation.External
{
	[Serializable]
	public readonly struct NeuroNode
	{
		public Vector3 Pos { get; }
		public float G { get; }
		public float H { get; }
		public float F => G + H;

		public NeuroNode(Vector3 current, Vector3 origin, Vector3 destination)
		{
			Pos = Round(current);
			G = Round(Vector3.Distance(origin, current));
			H = Round(Vector3.Distance(current, destination));
		}

		static Vector3 Round(Vector3 vector, int decimalPlaces = 3)
			=> new (
				Round(vector.x, decimalPlaces),
				Round(vector.y, decimalPlaces),
				Round(vector.z, decimalPlaces)
			);
		static float Round(float number, int decimalPlaces = 3)
			=> (float)Math.Round(number, decimalPlaces, MidpointRounding.AwayFromZero);
		public override string ToString()
			=> $"F:{F}, G:{G}, H:{H}, Pos:{Pos}";
	}
}