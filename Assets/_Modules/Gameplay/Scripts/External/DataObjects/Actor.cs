using Unity.Plastic.Newtonsoft.Json;
using System;


namespace Modules.Gameplay.External.DataObjects
{
	public class Actor
	{
		[JsonProperty("owned_by_user_id")]
		public Guid OwnedByUserId { get; set; }

		[JsonProperty("owner_by_team_id")]
		public Guid OwnedByTeamId { get; private set; }

		[JsonProperty("actor_id")]
		public Guid Id { get; }

		[JsonProperty("health")]
		public int Health { get; }

		[JsonProperty("x_coord")]
		public int X { get; set; }

		[JsonProperty("y_coord")]
		public int Y { get; set; }

		public Actor (int xCoord, int yCoord)
		{
			Id = Guid.NewGuid();
			Health = 100;
			X = xCoord;
			Y = yCoord;
		}

		public void SetOwnedByUser (User user)
		{
			OwnedByUserId = user.Id;
			OwnedByTeamId = user.Team.TeamId;
		}

		[JsonIgnore] public bool IsAlive => Health > 0;
		[JsonIgnore] public bool IsDead => !IsAlive;

		public bool IsOwnedByUser (User user) => OwnedByUserId == user.Id;
		public void SetCoords (int xCoord, int yCoord)
		{
			X = xCoord;
			Y = yCoord;
		}
	}
}