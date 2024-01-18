using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Internal.Schema
{
	[Serializable]
	public class MeshFilterDependencies
	{
		public MeshFilter MeshFilter;
		public Mesh Mesh;

		public MeshFilterDependencies(MeshFilter meshFilter, Mesh mesh)
		{
			MeshFilter = meshFilter;
			Mesh = mesh;
		}
	}
}