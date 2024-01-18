using UnityEngine.AddressableAssets;
using UnityEngine;
using System;

namespace Modules.Client.AssetManager.External.Schema
{
	[Serializable]
	public class HashedAvatarRef : AssetReferenceT<Avatar>
	{
		public string Hash { get; private set; }
		public HashedAvatarRef(string guid, string hash) : base(guid)
			=> Hash = hash;
	}
}