using UnityEditor;
using UnityEngine;

namespace Modules.Client.AssetManager.Internal
{
	public static class ModuleExportContextMenu
	{
		[MenuItem("GameObject/Modules/Asset Manager/Export To Module", false, -1)]
		static void ExportToModule()
		{
			var moduleData = Selection.activeGameObject.GetComponent<ModuleExportData>();


			if (string.IsNullOrWhiteSpace(moduleData.ModuleAssetsPath))
			{
				Debug.Log("<color=orange><b>>>> Please set the module assets path before exporting.</b></color>");
				return;
			}
			if (string.IsNullOrWhiteSpace(moduleData.AssetAuthor))
			{
				Debug.Log("<color=orange><b>>>> Please set the asset author before exporting.</b></color>");
				return;
			}
			if (string.IsNullOrWhiteSpace(moduleData.AssetPackName))
			{
				Debug.Log("<color=orange><b>>>> Please set the asset pack name before exporting.</b></color>");
				return;
			}

			Debug.Log($"<color=#00ffff><b>>>> Exporting {moduleData.name}...</b></color>");
		}

		[MenuItem("GameObject/Modules/Asset Manager/Export To Module", true)]
		static bool ValidateModule()
		{
			var obj = Selection.activeGameObject;
			if (obj == null) return false;
			var moduleData = obj.GetComponent<ModuleExportData>();
			return moduleData != null;
		}
	}
}