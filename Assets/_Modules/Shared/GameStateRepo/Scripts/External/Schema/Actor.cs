using Unity.Plastic.Newtonsoft.Json;
using System;

using Modules.Shared.HexMap.External.Schema;


namespace Modules.Shared.GameStateRepo.External.Schema
{
	public class Actor
	{
		[JsonProperty("owned_by_user_id")]
		public Guid OwnedByUserId { get; set; }

		[JsonProperty("owner_by_team_id")]
		public Guid OwnedByTeamId { get; private set; }

		[JsonProperty("actor_id")]
		public Guid Id { get; }

		[JsonProperty("actor_prefab_id")]
		public string PrefabId { get; set; }

		[JsonProperty("health")]
		public int Health { get; }

		[JsonProperty("ne_coord")]
		public double Ne { get; set; }

		[JsonProperty("se_coord")]
		public double Se { get; set; }

		public Actor (string prefabId, Hex2 coords)
		{
			PrefabId = prefabId;
			Id = Guid.NewGuid();
			Health = 100;
			Ne = (float)Math.Round(coords.ne, 3);
			Se = (float)Math.Round(coords.se, 3);
		}

		public Hex2 Coords => new Hex2(Ne, Se);

		public void SetOwnedByUser (User user)
		{
			OwnedByUserId = user.Id;
			OwnedByTeamId = user.Team.TeamId;
		}

		[JsonIgnore] public bool IsAlive => Health > 0;
		[JsonIgnore] public bool IsDead => !IsAlive;

		public bool IsOwnedByUser (User user) => OwnedByUserId == user.Id;
		public void SetCoords (Hex2 coords)
		{
			Ne = (float)Math.Round(coords.ne, 3);
			Se = (float)Math.Round(coords.se, 3);
		}
	}
}