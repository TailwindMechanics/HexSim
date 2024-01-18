using UnityEngine.AddressableAssets;
using UnityEngine;
using System;

namespace Modules.Client.AssetManager.External.Schema
{
	[Serializable]
	public class HashedMeshRef : AssetReferenceT<Mesh>
	{
		public string Hash { get; private set; }
		public HashedMeshRef(string guid, string hash) : base(guid)
			=> Hash = hash;
	}
}