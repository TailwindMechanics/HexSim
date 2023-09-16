using UnityEngine;

using Modules.HexMap.External.DataObjects;


namespace Modules.HexMap.External
{
	public static class Extensions
	{
		public static float NeDot (this Vector3 input)
			=> Vector3.Dot(input.normalized, Hex2.NeAxis.normalized);
		public static float SeDot (this Vector3 input)
			=> Vector3.Dot(input.normalized, Hex2.SeAxis.normalized);

		public static Hex2 ToHex2(this Vector3 point, Vector3 worldOrigin = default)
		{
			point.y = 0;
			var originToPoint = point - worldOrigin;
			var neDot = originToPoint.NeDot();
			var seDot = originToPoint.SeDot();

			var neIsCloser = Mathf.Abs(neDot) > Mathf.Abs(seDot);
			var hexDirectionA = neIsCloser ? Hex2.NeAxis : Hex2.SeAxis;
			var hexDirectionB = neIsCloser ? Hex2.SeAxis : Hex2.NeAxis;

			var intersectionA = LineLineIntersection(worldOrigin, hexDirectionA, point, hexDirectionB);
			var intersectionB = LineLineIntersection(worldOrigin, hexDirectionB, point, hexDirectionA);
			var signedMagnitudeA = Mathf.Sign(intersectionA.x) * intersectionA.magnitude;
			var signedMagnitudeB = Mathf.Sign(intersectionB.x) * intersectionB.magnitude;

			return new Hex2
			{
				ne = neIsCloser ? signedMagnitudeA : signedMagnitudeB,
				se = neIsCloser ? signedMagnitudeB : signedMagnitudeA
			};
		}

		public static Hex2 Round(this Hex2 input)
			=> new(Mathf.Round(input.ne), Mathf.Round(input.se));
		public static Hex2 Floor(this Hex2 input)
			=> new(Mathf.Floor(input.ne), Mathf.Floor(input.se));
		public static Hex2 Ceil(this Hex2 input)
			=> new(Mathf.Ceil(input.ne), Mathf.Ceil(input.se));

		static Vector3 LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			var lineVec3 = linePoint2 - linePoint1;
			var cross1 = Vector3.Cross(lineVec1, lineVec2);
			var cross2 = Vector3.Cross(lineVec3, lineVec2);
			var planarFactor = Vector3.Dot(lineVec3, cross1);

			if (Mathf.Abs(planarFactor) < 0.0001f && cross1.sqrMagnitude > 0.0001f)
			{
				var s = Vector3.Dot(cross2, cross1) / cross1.sqrMagnitude;
				return linePoint1 + lineVec1 * s;
			}

			return Vector3.zero;
		}

		public static Vector3 ToVector3(this Hex2 hex)
		{
			var hexDirA = hex.ne > hex.se ? Hex2.NeAxis : Hex2.SeAxis;
			var hexDirB = hex.ne > hex.se ? Hex2.SeAxis : Hex2.NeAxis;

			var origin = Vector3.zero;
			var hexMagA = hex.ne > hex.se ? hex.ne : hex.se;
			var pointA = origin + hexDirA * hexMagA;

			var hexMagB = hex.ne > hex.se ? hex.se : hex.ne;
			var pointB = origin + hexDirB * hexMagB;

			return (pointA + pointB) * -1;
		}
	}
}