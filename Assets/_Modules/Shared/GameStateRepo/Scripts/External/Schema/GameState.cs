using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Modules.Shared.GameStateRepo.External.Schema
{
	[Serializable]
	public class GameState
	{
		[JsonProperty("users")]
		public List<User> Users { get; }

		[JsonProperty("radius")]
		public int Radius { get; set; }

		[JsonProperty("seed")]
		public string Seed { get; set; }

		public GameState (int radius, string seed, List<User> users)
		{
			Radius = radius;
			Seed = seed;
			Users = users;
		}

		public User GetUserByTeamName (string teamName)
			=> Users.FirstOrDefault(user => user.Team.TeamName == teamName);

		public bool UserIsInAnyTeam (User query)
			=> Users.Any(user => user.Team.OwnerId == query.Id);
		public Actor GetActor (User query, Guid actorId)
			=> Users.FirstOrDefault(user => user.Team.IsOwnedByUser(query))
				?.Team.GetActor(query, actorId);
	}
}