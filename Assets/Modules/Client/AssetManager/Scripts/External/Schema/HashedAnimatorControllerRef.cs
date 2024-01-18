using UnityEngine.AddressableAssets;
using UnityEditor.Animations;
using System;

namespace Modules.Client.AssetManager.External.Schema
{
	[Serializable]
	public class HashedAnimatorControllerRef : AssetReferenceT<AnimatorController>
	{
		public string Hash { get; private set; }
		public HashedAnimatorControllerRef(string guid, string hash) : base(guid)
			=> Hash = hash;
	}
}