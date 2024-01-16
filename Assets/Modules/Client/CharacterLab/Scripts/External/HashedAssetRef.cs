using UnityEngine.AddressableAssets;
using UnityEngine;
using System;

namespace Modules.Client.CharacterLab.External
{
	[Serializable]
	public class HashedMaterialRef : AssetReferenceT<Material>
	{
		public HashedMaterialRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedTextureRef : AssetReferenceTexture
	{
		public HashedTextureRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedMeshRef : AssetReferenceT<Mesh>
	{
		public HashedMeshRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedAnimatorRef : AssetReferenceT<Animator>
	{
		public HashedAnimatorRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedMeshRendRef : AssetReferenceT<MeshRenderer>
	{
		public HashedMeshRendRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedSkinMeshRendRef : AssetReferenceT<SkinnedMeshRenderer>
	{
		public HashedSkinMeshRendRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedMeshFilterRef : AssetReferenceT<MeshFilter>
	{
		public HashedMeshFilterRef(string guid) : base(guid){}
	}

	[Serializable]
	public class HashedMeshColliderRef : AssetReferenceT<MeshCollider>
	{
		public HashedMeshColliderRef(string guid) : base(guid){}
	}
}