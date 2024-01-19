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

            AddToAddressablesGroup(assetSource, exportData.ModuleName);

            var prefabPath = $"{assetFolderPath}/{exportData.name}.prefab";
            clonedObject.AddComponent<AssetSourceData>().SetSourceData(
                new AssetSourceDataReference(AssetDatabase.AssetPathToGUID(newAssetSourcePath))
            );

            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                clonedObject,
                prefabPath,
                InteractionMode.AutomatedAction
            );

            AddToAddressablesGroup(prefab, exportData.ModuleName);

            assetSource.PopulateDependencies(prefab, exportData.AssetAuthor, exportData.AssetPackName, exportData.AssetsOriginalName);

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

        static void AddToAddressablesGroup(Object objectToAdd, string groupName)
        {
            // Get the Addressables settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // Check if the group exists
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                // Create a new group if it doesn't exist
                group = settings.CreateGroup(groupName,
                    false,
                    false,
                    false,
                    new List<AddressableAssetGroupSchema> {
                        settings.DefaultGroup.Schemas[0],
                        settings.DefaultGroup.Schemas[1]
                    }
                );

                // Add BundledAssetGroupSchema to the group
                var schema = group.AddSchema<BundledAssetGroupSchema>();
                schema.BuildPath.SetVariableByName(settings, "RemoteBuildPath");
                schema.LoadPath.SetVariableByName(settings, "RemoteLoadPath");
                schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            }

            // Create an AddressableAssetEntry for the GameObject
            var assetPath = AssetDatabase.GetAssetPath(objectToAdd);
            var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group);

            // Optionally set the address of the entry to the name of the GameObject for easy access
            entry.address = objectToAdd.name;

            // Save the settings
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        }

    }
}