using System.Linq;
using UnityEngine;


namespace Modules.Client.Utilities.External
{
	public static class StringExtensions
	{
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