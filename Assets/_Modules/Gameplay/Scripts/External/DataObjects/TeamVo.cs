using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using UnityEngine.Serialization;


namespace Modules.Gameplay.External.DataObjects
{
	[Serializable]
	public class TeamVo
	{
		public string OwnerUsername => ownerUsername;
		public string TeamName => teamName;
		public List<Vector2Int> ActorCoords => actorCoords;

		[FoldoutGroup("$TeamName"), Required, SerializeField] string teamName = "Blue";
		[FormerlySerializedAs("teamOwner")] [FoldoutGroup("$TeamName"), Required, SerializeField] string ownerUsername = "Username";
		[FoldoutGroup("$TeamName"), Required, SerializeField] GameObject actorPrefab;
		[FoldoutGroup("$TeamName"), RequiredListLength(1, 25),SerializeField]
		List<Vector2Int> actorCoords = new();
	}
}