using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	[Serializable]
	public class MeshRendererDependencies
	{
		public MeshRenderer MeshRenderer;
		public MaterialDependencies[] MaterialDependencies;

		public MeshRendererDependencies(MeshRenderer meshRenderer, MaterialDependencies[] materialDeps)
		{
			MeshRenderer = meshRenderer;
			MaterialDependencies = materialDeps;
		}
	}
}