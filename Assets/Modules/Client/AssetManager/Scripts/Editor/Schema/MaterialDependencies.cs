using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	[Serializable]
	public class MaterialDependencies
	{
		public Material Material;
		public Texture[] Maps;

		public MaterialDependencies(Material material, Texture[] maps)
		{
			Material = material;
			Maps = maps;
		}
	}
}