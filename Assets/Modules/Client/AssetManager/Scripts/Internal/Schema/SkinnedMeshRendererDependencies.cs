﻿using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Internal.Schema
{
	[Serializable]
	public class SkinnedMeshRendererDependencies
	{
		public SkinnedMeshRenderer SkinnedMeshRenderer;
		public Mesh Mesh;
		public Material[] Materials;

		public SkinnedMeshRendererDependencies(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh, Material[] materials)
		{
			SkinnedMeshRenderer = skinnedMeshRenderer;
			Mesh = mesh;
			Materials = materials;
		}
	}
}