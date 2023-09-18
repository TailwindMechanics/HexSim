using JetBrains.Annotations;
using Sirenix.OdinInspector;
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

		[FoldoutGroup("$GroupName"), SerializeField] string label = "Untitled";
		[FoldoutGroup("$GroupName"), SerializeField] Color color;
		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField] float minHeight;
		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField] float maxHeight;

		[UsedImplicitly]
		string GroupName => $"{label}, {color.ToHex()}: ({MinHeight}, {MaxHeight})";
	}
}