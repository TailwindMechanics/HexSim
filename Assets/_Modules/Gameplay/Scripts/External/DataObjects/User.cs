using Unity.Plastic.Newtonsoft.Json;
using System;


namespace Modules.Gameplay.External.DataObjects
{
	public class User
	{
		[JsonProperty("username")]
		public string Username { get; }

		[JsonProperty("user_id")]
		public Guid Id { get; }

		[JsonProperty("team")]
		public Team Team { get; }

		public User (string username, Team team)
		{
			Id = Guid.NewGuid();
			Username = username;
			Team = team;
		}
	}
}