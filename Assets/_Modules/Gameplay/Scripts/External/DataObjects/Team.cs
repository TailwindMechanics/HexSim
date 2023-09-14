using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System;


namespace Modules.Gameplay.External.DataObjects
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

		public Team (TeamVo teamSettings)
		{
			TeamId = Guid.NewGuid();
			TeamName = teamSettings.TeamName;
			Actors = new List<Actor>();
			teamSettings.ActorCoords.ForEach(coord =>
			{
				var newActor = new Actor(coord.x, coord.y);
				Actors.Add(newActor);
			});
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