using UnityEngine.AddressableAssets;
using UnityEngine;
using System;

namespace Modules.Client.AssetManager.External.Schema
{
	[Serializable]
	public class HashedMaterialRef : AssetReferenceT<Material>
	{
		public string Hash { get; private set; }
		public HashedMaterialRef(string guid, string hash) : base(guid)
			=> Hash = hash;
	}
}