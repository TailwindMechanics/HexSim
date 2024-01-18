using System.Collections.Generic;
using UnityEngine;

using Modules.Client.AssetManager.Internal.Schema;

namespace Modules.Client.AssetManager.Internal
{
	public class ModuleExportData : MonoBehaviour
	{
		public string ModuleAssetsPath => moduleAssetsPath;
		public string AssetAuthor => assetAuthor;
		public string AssetPackName => assetPackName;

		[TextArea(1,1), SerializeField] string moduleAssetsPath;
		[TextArea(1,1), SerializeField] string assetAuthor;
		[TextArea(1,1), SerializeField] string assetPackName;


		[SerializeField] List<string> unknownComponentTypes = new();

		[Header("# Asset Dependencies")]
		public List<AnimatorDependencies> animators = new();
		public List<MeshFilterDependencies> meshFilters = new();
		public List<MeshRendererDependencies> meshRenderers = new();
		public List<SkinnedMeshRendererDependencies> skinMeshRenderers = new();

	}
}