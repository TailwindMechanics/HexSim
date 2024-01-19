using UnityEngine;

using Modules.Client.AssetManager.External.Schema;

namespace Modules.Client.AssetManager.External
{
	public class AssetSourceData : MonoBehaviour
	{
		public void SetSourceData(AssetSourceDataReference source)
			=> sourceData = source;
		[SerializeField] AssetSourceDataReference sourceData;
	}
}