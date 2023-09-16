using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


namespace Modules.Gameplay.External.DataObjects
{
	[Serializable]
	public class GameState
	{
		[JsonProperty("users")]
		public List<User> Users { get; } = new();

		[JsonProperty("seed")]
		public string Seed { get; set; }

		public User GetUserByTeamName (string teamName)
			=> Users.FirstOrDefault(user => user.Team.TeamName == teamName);

		public GameState AddUser (User user)
		{
			Users.Add(user);
			return this;
		}

		public bool UserIsInAnyTeam (User query)
			=> Users.Any(user => user.Team.OwnerId == query.Id);
		public Actor GetActor (User query, Guid actorId)
			=> Users.FirstOrDefault(user => user.Team.IsOwnedByUser(query))
				?.Team.GetActor(query, actorId);
		public void SetActorCoords(User user, Guid actorId, Vector2Int newCoords)
			=> GetActor(user, actorId)?.SetCoords(newCoords.x, newCoords.y);
	}
}