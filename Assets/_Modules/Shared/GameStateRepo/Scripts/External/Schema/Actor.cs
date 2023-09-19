using Unity.Plastic.Newtonsoft.Json;
using System;

using Modules.Shared.HexMap.External.Schema;


namespace Modules.Shared.GameStateRepo.External.Schema
{
	public class Actor
	{
		[JsonProperty("owned_by_user_id")]
		public Guid OwnedByUserId { get; private set; }

		[JsonProperty("owner_by_team_id")]
		public Guid OwnedByTeamId { get; private set; }

		[JsonProperty("actor_id")]
		public Guid Id { get; }

		[JsonProperty("actor_prefab_id")]
		public string PrefabId { get; }

		[JsonProperty("health")]
		public int Health { get; private set; }

		[JsonProperty("hit_points")]
		public int HitPoints { get; private set; }

		[JsonProperty("ne_coord")]
		public double Ne { get; set; }

		[JsonProperty("se_coord")]
		public double Se { get; set; }

		public Actor (string prefabId, Hex2 coords, int hitPoints)
		{
			PrefabId = prefabId;
			Id = Guid.NewGuid();
			Health = 100;
			Ne = (float)Math.Round(coords.ne, 3);
			Se = (float)Math.Round(coords.se, 3);
			HitPoints = hitPoints;
		}

		public void SetOwnedByUser (User user)
		{
			OwnedByUserId = user.Id;
			OwnedByTeamId = user.Team.TeamId;
		}

		public void SetCoords (Hex2 coords)
		{
			Ne = (float)Math.Round(coords.ne, 3);
			Se = (float)Math.Round(coords.se, 3);
		}

		public bool IsDead
			=> Health < 1;
		public Hex2 Coords
			=> new(Ne, Se);
		public void DecrementHealth (int decrement)
			=> Health -= decrement;
		public bool IsOwnedByUser (Guid id)
			=> OwnedByUserId == id;
	}
}