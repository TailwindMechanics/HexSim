using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;

using Modules.Client.AssetManager.Editor.Schema;
using Modules.Client.AssetManager.External;
using Modules.Client.AssetManager.Internal;

namespace Modules.Client.AssetManager.Editor
{
    public static class ModuleExportContextMenu
    {
        [MenuItem("GameObject/Modules/Asset Manager/Export To Module", false, -1)]
        static void ExportToModule()
        {
            var exportData = Selection.activeGameObject.GetComponent<ModuleExportData>();

            if (string.IsNullOrWhiteSpace(exportData.ModuleName))
            {
                Debug.Log("<color=orange><b>>>> Please set the module name before exporting.</b></color>");
                return;
            }
            if (string.IsNullOrWhiteSpace(exportData.ModuleAssetsPath))
            {
                Debug.Log("<color=orange><b>>>> Please set the module assets path before exporting.</b></color>");
                return;
            }
            if (string.IsNullOrWhiteSpace(exportData.AssetAuthor))
            {
                Debug.Log("<color=orange><b>>>> Please set the asset author before exporting.</b></color>");
                return;
            }
            if (string.IsNullOrWhiteSpace(exportData.AssetPackName))
            {
                Debug.Log("<color=orange><b>>>> Please set the asset pack name before exporting.</b></color>");
                return;
            }
            if (string.IsNullOrWhiteSpace(exportData.AssetsOriginalName))
            {
                Debug.Log("<color=orange><b>>>> Please set the asset's original name before exporting.</b></color>");
                return;
            }

            var assetFolderPath = $"{exportData.ModuleAssetsPath}/{exportData.name}";
            if (!AssetDatabase.IsValidFolder(assetFolderPath))
            {
                AssetDatabase.CreateFolder(exportData.ModuleAssetsPath, exportData.name);
            }

            if (exportData.gameObject.activeSelf == false)
            {
                exportData.gameObject.SetActive(true);
            }

            var clonedObject = Object.Instantiate(exportData.gameObject);
            clonedObject.name = exportData.name;
            Object.DestroyImmediate(clonedObject.GetComponent<ModuleExportData>());
            exportData.gameObject.SetActive(false);

            var assetSource = ScriptableObject.CreateInstance<AssetSourceDataSo>();
            var newAssetSourcePath = $"{assetFolderPath}/{clonedObject.name}_assetSource.asset";

            AssetDatabase.CreateAsset(assetSource, newAssetSourcePath);

            AddressableUtil.AddToAddressablesGroup(assetSource, exportData.ModuleName);

            var prefabPath = $"{assetFolderPath}/{exportData.name}.prefab";
            clonedObject.AddComponent<AssetSourceData>().SetSourceData(
                new AssetSourceDataReference(AssetDatabase.AssetPathToGUID(newAssetSourcePath))
            );

            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                clonedObject,
                prefabPath,
                InteractionMode.AutomatedAction
            );

            AddressableUtil.AddToAddressablesGroup(prefab, exportData.ModuleName);

            assetSource.CloneDependencies(prefab, exportData.ModuleName, exportData.AssetAuthor, exportData.AssetPackName, exportData.AssetsOriginalName);

            Debug.Log($"<color=#00ffff><b>>>> Exported {prefab.name}.</b></color>");

            Selection.activeGameObject = clonedObject;

            AssetDatabase.Refresh();
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