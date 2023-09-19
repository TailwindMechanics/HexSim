using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Modules.Client.Actors.External.Schema
{
	[CreateAssetMenu(fileName = "new _actorPrefabMapping", menuName = "Modules/Actors/Prefab Map")]
	public class ActorPrefabMappingSo : ScriptableObject
	{
		public GameObject GetPrefabById(string id)
			=> actorPrefabPairs.FirstOrDefault(pair => pair.Vo.PrefabId == id)?.Vo.Prefab;
		[SerializeField] List<ActorPrefabPairSo> actorPrefabPairs;
	}
}