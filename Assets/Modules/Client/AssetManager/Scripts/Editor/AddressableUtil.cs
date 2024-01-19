using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modules.Client.AssetManager.Editor
{
	public static class AddressableUtil
	{
		public static void AddToAddressablesGroup(Object objectToAdd, string groupName)
		{
			if (objectToAdd == null) return;

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