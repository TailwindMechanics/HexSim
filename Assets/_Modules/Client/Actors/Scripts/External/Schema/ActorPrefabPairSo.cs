﻿using UnityEngine;


namespace Modules.Client.Actors.External.Schema
{
	[CreateAssetMenu(fileName = "new _actorPrefabPair", menuName = "Modules/Actors/Prefab Pair")]
	public class ActorPrefabPairSo : ScriptableObject
	{
		public ActorPrefabPairVo Vo => pair;
		[SerializeField] ActorPrefabPairVo pair;
	}
}