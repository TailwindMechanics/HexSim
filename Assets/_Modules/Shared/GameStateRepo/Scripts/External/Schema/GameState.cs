using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

using Modules.Shared.HexMap.External.Schema;


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

		[JsonProperty("min_walk_height")]
		public float MinWalkHeight { get; set; }

		[JsonProperty("amplitude")]
		public float Amplitude { get; set; }

		[JsonProperty("noise_scale")]
		public float NoiseScale { get; set; }

		[JsonProperty("noise_offset_x")]
		public int NoiseOffsetX { get; set; }

		[JsonProperty("noise_offset_y")]
		public int NoiseOffsetY { get; set; }

		[JsonIgnore]
		public HexGrid Grid { get; private set; }

		public GameState (HexGrid grid, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY, List<User> users)
		{
			Radius = radius;
			Seed = seed;
			MinWalkHeight = minWalkHeight;
			Amplitude = amplitude;
			NoiseScale = noiseScale;
			NoiseOffsetX = noiseOffsetX;
			NoiseOffsetY = noiseOffsetY;
			Users = users;
			Grid = grid;
		}

		public Team GetTeamById (Guid teamId)
			=> Users.FirstOrDefault(user => user.Team.TeamId == teamId)?.Team;
		public float SeedAsFloat
			=> SeedToFloat(Seed);
		public User GetUserByTeamName (string teamName)
			=> Users.FirstOrDefault(user => user.Team.TeamName == teamName);
		public bool UserIsInAnyTeam (User query)
			=> Users.Any(user => user.Team.OwnerId == query.Id);
		public Actor GetActor (User query, Guid actorId)
			=> Users.FirstOrDefault(user => user.Team.IsOwnedByUser(query))
				?.Team.GetActor(query, actorId);

		float SeedToFloat(string str)
		{
			var seed = str.Aggregate<char, long>(0, (current, c) => (current << 5) - current + c);
			const int prime = 1000000007;
			return (float)(seed % prime) / prime;
		}
	}
}