using UnityEngine;


namespace Modules.Client.Utilities.External
{
	public static class ColorExtensions
	{
		public static string ToHex(this Color color)
			=>  $"#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}";
	}
}