using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.Shared.GameStateRepo.External.Schema
{
	public class Team
	{
		[JsonProperty("owner_user_id")]
		public Guid OwnerId { get; private set; }

		[JsonProperty("team_id")]
		public Guid TeamId { get; private set; }

		[JsonProperty("team_name")]
		public string TeamName { get; }

		[JsonProperty("actors")]
		public List<Actor> Actors { get; }

		public Team (string teamName, List<Actor> actors)
		{
			TeamId = Guid.NewGuid();
			TeamName = teamName;
			Actors = actors;
		}

		public void SetOwner (User owner)
		{
			OwnerId = owner.Id;
			Actors.ForEach(actor => actor.SetOwnedByUser(owner));
		}

		public bool IsOwnedByUser (User user)
			=> OwnerId == user.Id;
		public Actor GetActor (User user, Guid actorId)
			=> Actors.Find(actor => actor.Id == actorId);
	}
}