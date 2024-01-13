using JetBrains.Annotations;
using UnityEngine;
using System;


namespace Modules.Client.Actors.External.Schema
{
	[Serializable]
	public class ActorPrefabPairVo
	{
		public string PrefabId => prefabId;
		public GameObject Prefab => prefab;

		[SerializeField] string prefabId;
		[SerializeField] GameObject prefab;

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