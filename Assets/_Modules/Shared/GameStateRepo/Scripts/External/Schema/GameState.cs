using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using Modules.Client.HexTiles.External.Schema;


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

		[JsonProperty("amplitude")]
		public float Amplitude { get; set; }

		[JsonProperty("height_color")]
		public HeightColorMapVo HeightColorMap { get; set; }

		[JsonProperty("noise_scale")]
		public float NoiseScale { get; set; }

		[JsonProperty("noise_offset_x")]
		public int NoiseOffsetX { get; set; }

		[JsonProperty("noise_offset_y")]
		public int NoiseOffsetY { get; set; }

		public GameState (int radius, string seed, HeightColorMapVo heightColorMap, float amplitude, float noiseScale, Vector2Int noiseOffset, List<User> users)
		{
			Radius = radius;
			Seed = seed;
			Amplitude = amplitude;
			NoiseScale = noiseScale;
			NoiseOffsetX = noiseOffset.x;
			NoiseOffsetY = noiseOffset.y;
			HeightColorMap = heightColorMap;
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