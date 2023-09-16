using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.HexTiles.Internal.DataObjects
{
	[Serializable]
	public class EdgeLoop
	{
		const float scaleFactor = 0.575f;
		public float InnerRadius => innerRadius * scaleFactor;
		public float InnerHeight => innerHeight;
		public float OuterRadius => outerRadius * scaleFactor;
		public float OuterHeight => outerHeight;
		public bool Reverse => reverse;


		[FoldoutGroup("$GroupName"), SerializeField]
		string label = "Untitled";

		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
		float innerRadius = .9f;
		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
		float outerRadius = .9f;

		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
		float innerHeight = 1f;
		[FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
		float outerHeight = 1f;
		[FoldoutGroup("$GroupName"), SerializeField]
		bool reverse;

		[UsedImplicitly]
		string GroupName => string.IsNullOrEmpty(label) ? "Untitled" : label;
	}
}