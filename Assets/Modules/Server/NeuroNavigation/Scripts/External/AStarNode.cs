using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Modules.Server.NeuroNavigation.External
{
	[Serializable]
	public readonly struct NeuroNode
	{
		public Vector3 Pos { get; }
		public float B { get; }
		public float G { get; }
		public float H { get; }
		// public float F => (G + H) / B;
		public float F => G + H;

		public NeuroNode(Vector3 current, Vector3 origin, Vector3 destination, IEnumerable<float> bonus = null)
		{
			Pos = Round(current);
			G = Round(Vector3.Distance(origin, current));
			H = Round(Vector3.Distance(current, destination));
			B = bonus != null ? Round(bonus.Sum()) : 0f;
		}

		static float Round(float number, int decimalPlaces = 3)
		{
			return (float)Math.Round(number, decimalPlaces, MidpointRounding.AwayFromZero);
		}

		static Vector3 Round(Vector3 vector, int decimalPlaces = 3)
		{
			return new Vector3(
				Round(vector.x, decimalPlaces),
				Round(vector.y, decimalPlaces),
				Round(vector.z, decimalPlaces)
			);
		}

		public override string ToString()
			=> $"F:{F}, B:{B}, G:{G}, H:{H}, Pos:{Pos}";
	}
}