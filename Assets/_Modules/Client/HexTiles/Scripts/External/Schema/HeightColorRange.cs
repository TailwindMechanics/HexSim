using JetBrains.Annotations;
using UnityEngine;
using System;

using Modules.Client.Utilities.External;


namespace Modules.Client.HexTiles.External.Schema
{
	[Serializable]
	public class HeightColorRange
	{
		public Color? GetColorInRange(float height)
			=> IsHeightInRange(height) ? color : null;
		bool IsHeightInRange(float height)
			=> height >= MinHeight && height <= MaxHeight;

		public Color Color => color;
		public float MinHeight => minHeight;
		public float MaxHeight => maxHeight;

		[SerializeField] string label = "Untitled";
		[SerializeField] Color color;
		[Range(0f, 1f), SerializeField] float minHeight;
		[Range(0f, 1f), SerializeField] float maxHeight;

		[UsedImplicitly]
		string GroupName => $"{label}, {color.ToHex()}: ({MinHeight}, {MaxHeight})";
	}
}