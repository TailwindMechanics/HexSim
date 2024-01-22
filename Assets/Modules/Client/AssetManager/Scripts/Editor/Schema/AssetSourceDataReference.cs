using UnityEngine.AddressableAssets;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	[Serializable]
	public class AssetSourceDataReference : AssetReferenceT<AssetSourceDataSo>
	{
		public AssetSourceDataReference(string guid) : base(guid){}
	}
}