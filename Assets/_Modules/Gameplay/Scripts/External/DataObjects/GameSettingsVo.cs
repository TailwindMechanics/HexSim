using UnityEngine;
using System;

using System.Collections.Generic;


namespace Modules.Gameplay.External.DataObjects
{
	[Serializable]
	public class GameSettingsVo
	{
		public int GridRadius => gridRadius;
		public List<TeamVo> Teams => teams;
		public string Seed => seed;

		[SerializeField] string seed = "seed";
		[Range(0, 50), SerializeField] int gridRadius = 5;
		[SerializeField] List<TeamVo> teams = new();
	}
}