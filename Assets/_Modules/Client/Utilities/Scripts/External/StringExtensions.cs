using System.Linq;
using UnityEngine;


namespace Modules.Client.Utilities.External
{
	public static class StringExtensions
	{
		public static float ToSeedFloat(this string str)
		{
			var seed = str.Aggregate<char, long>(0, (current, c) => (current << 5) - current + c);
			const int prime = 1000000007;
			return (float)(seed % prime) / prime;
		}

		public static Color HexToColor(this string hex)
		{
			if (hex.StartsWith("#"))
			{
				hex = hex.Substring(1);
			}

			if (hex.Length != 6)
			{
				throw new System.ArgumentException("Invalid hex string length. Expected exactly 6 characters.");
			}

			var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color(r / 255f, g / 255f, b / 255f);
		}
	}
}