using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;

using Modules.Shared.Neese.HexTwo.External.Schema;

namespace Modules.Shared.GameStateRepo.External.Schema
{
	[Serializable]
	public class GameState
	{
		[JsonProperty("winning_team_name")]
		public string WinningTeamName { get; set; }

		[JsonProperty("users")]
		public List<User> Users { get; }

		[JsonProperty("player_pos_ne")]
		public float PlayerPosNe { get; set; }

		[JsonProperty("player_pos_se")]
		public float PlayerPosSe { get; set; }

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


		public GameState (HexGrid grid, Hex2 playerPos, int radius, string seed, float minWalkHeight, float amplitude, float noiseScale, int noiseOffsetX, int noiseOffsetY, List<User> users)
		{
			Radius = radius;
			PlayerPosNe = playerPos.ne;
			PlayerPosSe = playerPos.se;
			Seed = seed;
			MinWalkHeight = minWalkHeight;
			Amplitude = amplitude;
			NoiseScale = noiseScale;
			NoiseOffsetX = noiseOffsetX;
			NoiseOffsetY = noiseOffsetY;
			Users = users;
			Grid = grid;
		}

		public void SetPlayerPos (Hex2 newCoord)
		{
			newCoord = newCoord.Round();
			PlayerPosNe = newCoord.ne;
			PlayerPosSe = newCoord.se;
		}

		public User GetWinner()
		{
			if (Users.Count < 2) return null;

			var survivingUsers = Users.Where(user => user.Team.Actors.Any(actor => !actor.IsDead)).ToList();
			return survivingUsers.Count < 2 ? survivingUsers[0] : null;
		}

		float SeedToFloat(string str)
		{
			var seed = str.Aggregate<char, long>(0, (current, character)
				=> (current << 5) - current + character);
			const int prime = 1000000007;
			return (float)(seed % prime) / prime;
		}

		public void GameOver (Team team)
			=> WinningTeamName = team.TeamName;
		public List<Actor> ActorsAtCoord (Hex2 coord)
			=> Users.SelectMany(user => user.Team.Actors)
				.Where(actor => actor.Coords == coord)
				.ToList();
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
	}
}