using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Modules.Server.AStarNav.External
{
	[Serializable]
	public readonly struct AStarNode
	{
		public Vector3 Pos { get; }
		public float[] B { get; }
		public float G { get; }
		public float H { get; }
		public float F => G + H + B.Sum();

		public AStarNode(int currentGivenSteps, Vector3 current, Vector3 origin, Vector3 destination, Func<Vector3, List<float>> costsAtPos)
		{
			Pos = Round(current);
			G = currentGivenSteps;
			H = Round(Vector3.Distance(current, destination));
			B = costsAtPos(Pos).ToArray();
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