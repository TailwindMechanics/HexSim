using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Modules.Client.AssetManager.External
{
	public class AssetSourceData : MonoBehaviour
	{
		public void SetSourceData(AssetReference source)
			=> sourceData = source;
		[SerializeField] AssetReference sourceData;
	}
}