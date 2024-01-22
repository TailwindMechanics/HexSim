using UnityEngine;

namespace Modules.Client.AssetManager.Internal
{
	public class ModuleExportData : MonoBehaviour
	{
		public string ModuleName => moduleName;
		public string PrefabName => prefabName;
		public string ModuleAssetsPath => moduleAssetsPath;
		public string AssetAuthor => assetAuthor;
		public string AssetPackName => assetPackName;
		public string AssetsOriginalName => assetsOriginalName;

		[TextArea(1,1), SerializeField] string moduleName;
		[TextArea(1,1), SerializeField] string prefabName;
		[TextArea(1,1), SerializeField] string moduleAssetsPath;
		[TextArea(1,1), SerializeField] string assetAuthor;
		[TextArea(1,1), SerializeField] string assetPackName;
		[TextArea(1,1), SerializeField] string assetsOriginalName;
	}
}