using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;

namespace Modules.Client.AssetManager.Internal
{
    public class AssetManagerTesting : MonoBehaviour
    {
        [SerializeField] string groupName;
        [SerializeField] GameObject root;

        void Start()
        {
            AddToAddressablesGroup(root, groupName);
        }

        void AddToAddressablesGroup(GameObject objectToAdd, string groupName)
        {
            // Get the Addressables settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // Check if the group exists
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                // Create a new group if it doesn't exist
                group = settings.CreateGroup(groupName, false, false, true, null);
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