using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.HexTiles.Internal.DataObjects
{
	[Serializable]
	public class TilePrefabVo
	{
		public GameObject Prefab => prefab;
		public Vector2 HeightRange => heightRange01;

		[FoldoutGroup("$GroupName"), SerializeField]
		string label = "Untitled";
		[FoldoutGroup("$GroupName"), SerializeField]
		GameObject prefab;
		[FoldoutGroup("$GroupName"), SerializeField]
		Vector2 heightRange01;

		[UsedImplicitly]
		string GroupName => string.IsNullOrEmpty(label) ? "Untitled" : label;
	}
}