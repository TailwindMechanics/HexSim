using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.Client.Actors.External.Schema
{
	[Serializable]
	public class ActorPrefabPairVo
	{
		public string PrefabId => prefabId;
		public GameObject Prefab => prefab;

		[FoldoutGroup("$GroupName"), SerializeField] string prefabId;
		[FoldoutGroup("$GroupName"), SerializeField] GameObject prefab;

		[UsedImplicitly]
		string GroupName => $"{Id}, {PrefabName}";
		[UsedImplicitly]
		string Id => !string.IsNullOrWhiteSpace(prefabId)
			? prefabId
			: "No id set";
		[UsedImplicitly]
		string PrefabName => prefab != null
			? prefab.name
			: "Unassigned";
	}
}