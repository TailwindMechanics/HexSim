using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.Client.GameSetup.External.Schema
{
	[Serializable]
	public class TeamVo
	{
		public List<ActorVo> Actors => actors;
		public string OwnerUsername => ownerUsername;
		public string TeamName => teamName;

		[FoldoutGroup("$TeamName"), Required, SerializeField] string teamName = "Blue";
		[FoldoutGroup("$TeamName"), Required, SerializeField] string ownerUsername = "Username";
		[FoldoutGroup("$TeamName"), RequiredListLength(1, 25),SerializeField]
		List<ActorVo> actors = new();
	}
}