using UnityEngine.AddressableAssets;
using System;

namespace Modules.Client.AssetManager.External.Schema
{
	[Serializable]
	public class HashedTextureRef : AssetReferenceTexture
	{
		public string Hash { get; private set; }
		public HashedTextureRef(string guid, string hash) : base(guid)
			=> Hash = hash;
	}
}