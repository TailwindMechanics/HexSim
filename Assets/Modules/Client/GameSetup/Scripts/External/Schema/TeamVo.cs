using System.Collections.Generic;
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

		[SerializeField] string teamName = "Blue";
		[SerializeField] string ownerUsername = "Username";
		[SerializeField]
		List<ActorVo> actors = new();
	}
}