using System.Collections.Generic;
using UnityEngine;
using System;

using Modules.Client.HexTiles.External.Schema;


namespace Modules.Client.GameSetup.External.Schema
{
	[Serializable]
	public class GameSettingsVo
	{
		public Vector2Int NoiseOffset => new(noiseOffsetX, noiseOffsetY);
		public HeightColorMapVo HeightColorMap => heightColorMap.Vo;
		public float MinWalkHeight => minWalkHeight;
		public float NoiseScale => noiseScale;
		public float Amplitude => amplitude;
		public int GridRadius => gridRadius;
		public List<TeamVo> Teams => teams;
		public string Seed => seed;

		[SerializeField] string seed = "seed";
		[Range(0, 50), SerializeField] int gridRadius = 5;
		[Range(0f, 1f), SerializeField] float minWalkHeight = .8f;
		[Range(.01f, 10f), SerializeField] float amplitude = 1.5f;
		[Range(.01f, 10f), SerializeField] float noiseScale = .05f;
		[Range(1, 5000), SerializeField] int noiseOffsetX = 1000;
		[Range(1, 5000), SerializeField] int noiseOffsetY = 2000;
		[SerializeField] HeightColorMapSo heightColorMap;
		[SerializeField] List<TeamVo> teams = new();
	}
}