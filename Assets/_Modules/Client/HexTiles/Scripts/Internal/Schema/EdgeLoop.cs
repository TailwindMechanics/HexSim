using JetBrains.Annotations;
using UnityEngine;
using System;


namespace Modules.Client.HexTiles.Internal.Schema
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


		[SerializeField]
		string label = "Untitled";

		[Range(0f, 1f), SerializeField]
		float innerRadius = .9f;
		[Range(0f, 1f), SerializeField]
		float outerRadius = .9f;

		[Range(0f, 1f), SerializeField]
		float innerHeight = 1f;
		[Range(0f, 1f), SerializeField]
		float outerHeight = 1f;
		[SerializeField]
		bool reverse;

		[UsedImplicitly]
		string GroupName => string.IsNullOrEmpty(label) ? "Untitled" : label;
	}
}