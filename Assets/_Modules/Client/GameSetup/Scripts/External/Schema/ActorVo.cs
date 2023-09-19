using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

using Modules.Client.Actors.External.Schema;
using Modules.Shared.HexMap.External.Schema;


namespace Modules.Client.GameSetup.External.Schema
{
	[Serializable]
	public class ActorVo
	{
		public ActorPrefabPairVo ActorPrefab => actorPrefab.Vo;
		public int HitPoints => hitPoints;
		public Hex2 Hex2Coords => hex2Coords;

		[FoldoutGroup("$GroupName"), SerializeField]
		ActorPrefabPairSo actorPrefab;
		[FoldoutGroup("$GroupName"), Range(0, 101), SerializeField]
		int hitPoints = 10;
		[FoldoutGroup("$GroupName"), SerializeField]
		Hex2 hex2Coords;

		[UsedImplicitly]
		string GroupName => $"{Id}, {hex2Coords}, hit points:{hitPoints}";
		[UsedImplicitly]
		string Id => actorPrefab != null
			? actorPrefab.Vo.PrefabId
			: "No id set";
	}
}