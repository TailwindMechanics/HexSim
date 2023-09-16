using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.Gameplay.External.DataObjects
{
	[Serializable]
	public class TeamVo
	{
		public List<Vector2Int> ActorCoords => actorCoords;
		public string OwnerUsername => ownerUsername;
		public GameObject ActorPrefab => actorPrefab;
		public string TeamName => teamName;

		[FoldoutGroup("$TeamName"), Required, SerializeField] string teamName = "Blue";
		[FoldoutGroup("$TeamName"), Required, SerializeField] string ownerUsername = "Username";
		[FoldoutGroup("$TeamName"), Required, SerializeField] GameObject actorPrefab;
		[FoldoutGroup("$TeamName"), RequiredListLength(1, 25),SerializeField]
		List<Vector2Int> actorCoords = new();
	}
}