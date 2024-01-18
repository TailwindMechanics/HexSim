using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Internal.Schema
{
	[Serializable]
	public class MeshRendererDependencies
	{
		public MeshRenderer MeshRenderer;
		public Material[] Materials;

		public MeshRendererDependencies(MeshRenderer meshRenderer, Material[] materials)
		{
			MeshRenderer = meshRenderer;
			Materials = materials;
		}
	}
}