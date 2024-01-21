using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	[Serializable]
	public class SkinnedMeshRendererDependencies
	{
		public SkinnedMeshRenderer SkinnedMeshRenderer;
		public Mesh Mesh;
		public MaterialDependencies[] MaterialDependencies;

		public SkinnedMeshRendererDependencies(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh, MaterialDependencies[] materialDeps)
		{
			SkinnedMeshRenderer = skinnedMeshRenderer;
			Mesh = mesh;
			MaterialDependencies = materialDeps;
		}
	}
}