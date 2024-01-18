using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Text;
using UnityEngine;
using System.IO;
using System;

using Modules.Client.EditorTools.Internal.Editor;
using Modules.Client.Utilities.External;

namespace Modules.Client.AssetManager.External
{
	[CreateAssetMenu(fileName = "new _assetManifest", menuName = "Modules/AssetManager/Manifest")]
	public class AssetManifestSo : ScriptableObject
	{
		[Header("# Settings")]
		[TextArea(1, 1), SerializeField] string externalAssetsPath;
		[TextArea(1, 1), SerializeField] string internalAssetsPath;
		[SerializeField] List<GameObject> prefabsToImport = new();
		[SerializeField] List<string> unknownComponentTypes = new();

		[Header("# Asset Dependencies")]
		public List<RuntimeAnimatorController> animatorControllers;
		public List<Avatar> avatars;
		public List<Mesh> meshes;
		public List<Material> materials;

		// [Header("# Found Components")]
		[HideInInspector] public List<Animator> animators;
		[HideInInspector] public List<MeshCollider> meshColliders;
		[HideInInspector] public List<MeshFilter> meshFilters;
		[HideInInspector] public List<MeshRenderer> meshRenderers;
		[HideInInspector] public List<SkinnedMeshRenderer> skinMeshRenderers;

		[Header("# Module Asset Folder")]

		[TextArea(10, 20), SerializeField] string fileTree;

		// void OnEnable()
		// 	=> EditorSave.OnSaved.AddListener(OnSaved);
		//
		// void OnDisable()
		// 	=> EditorSave.OnSaved.RemoveListener(OnSaved);

		// void OnValidate()
		// 	=> ImportOnChange();

		// void OnSaved()
		// 	=> ImportOnChange();

		public void AddToAddressablesGroup(GameObject gameObject, string groupName)
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
			var assetPath = AssetDatabase.GetAssetPath(gameObject);
			var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group);

			// Optionally set the address of the entry to the name of the GameObject for easy access
			entry.address = gameObject.name;

			// Save the settings
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
		}

		void ImportOnChange()
		{
			if (string.IsNullOrWhiteSpace(externalAssetsPath)) return;
			if (!FileTreeHasChanged()) return;

			Debug.Log($"<color=white><b>>>> {name}: File tree has changed, importing.</b></color>");
			Import();
		}

		bool FileTreeHasChanged()
		{
			var newFileTree = GetFileTree(externalAssetsPath);
			if (fileTree != newFileTree)
			{
				fileTree = newFileTree;
				return true;
			}

			return false;
		}

		string GetFileTree(string rootPath)
		{
			var sb = new StringBuilder();
			BuildFileTree(rootPath, sb, 0);
			return sb.ToString();
		}


		void BuildFileTree(string currentPath, StringBuilder sb, int indentLevel)
		{
			if (!Directory.Exists(currentPath))
			{
				Debug.Log($"<color=red><b>>>> {name}: Directory does not exist: {currentPath}</b></color>");
				return;
			}

			var entries = Directory.GetFileSystemEntries(currentPath)
				.Where(entry => !Path.GetFileName(entry).EndsWith(".meta"))
				.ToArray();

			for (var i = 0; i < entries.Length; i++)
			{
				var entry = entries[i];
				var entryName = Path.GetFileName(entry);

				// Create indentation for current entry
				var indent = new StringBuilder();
				for (var j = 0; j < indentLevel; j++)
				{
					indent.Append("│    ");
				}

				// Add branching symbol
				indent.Append(i < entries.Length - 1 ? "├─ " : "└─ ");

				// Append the entry name
				sb.AppendLine($"{indent}{entryName}");

				// Recursively build tree for directories
				if (Directory.Exists(entry))
				{
					BuildFileTree(entry, sb, indentLevel + 1);
				}
			}
		}

		void Import()
		{
			animatorControllers.Clear();
			avatars.Clear();
			meshes.Clear();
			materials.Clear();

			animators.Clear();
			meshColliders.Clear();
			meshFilters.Clear();
			meshRenderers.Clear();
			skinMeshRenderers.Clear();

			unknownComponentTypes.Clear();

			if (prefabsToImport.Count == 0)
			{
				Debug.Log($"<color=red><b>>>> {name}: No root assets to import</b></color>");
				return;
			}

			var components = new List<Component>();
			foreach (var root in prefabsToImport)
			{
				components.AddRange(root.GetComponentsInChildren<Component>(true));
			}

			foreach (var component in components)
			{
				var componentType = component.GetType();
				if (componentType == typeof(Animator))
				{
					var animator = component as Animator;
					if (animator != null)
					{
						if (animator.runtimeAnimatorController != null)
						{
							animatorControllers.AddUnique(animator.runtimeAnimatorController);
						}

						if (animator.avatar != null)
						{
							avatars.AddUnique(animator.avatar);
						}
					}

					animators.Add(component as Animator);
				}
				else if (componentType == typeof(MeshCollider))
				{
					meshColliders.Add(component as MeshCollider);
				}
				else if (componentType == typeof(MeshFilter))
				{
					var meshFilter = component as MeshFilter;
					if (meshFilter != null && meshFilter.sharedMesh != null)
					{
						meshes.AddUnique(meshFilter.sharedMesh);
						meshFilters.Add(meshFilter);
					}
				}
				else if (componentType == typeof(MeshRenderer))
				{
					var meshRenderer = component as MeshRenderer;
					if (meshRenderer != null && meshRenderer.sharedMaterials != null)
					{
						foreach (var material in meshRenderer.sharedMaterials)
						{
							if (material != null)
							{
								materials.AddUnique(material);
							}
						}
					}

					meshRenderers.Add(meshRenderer);
				}
				else if (componentType == typeof(SkinnedMeshRenderer))
				{
					var skinnedMeshRenderer = component as SkinnedMeshRenderer;
					if (skinnedMeshRenderer != null)
					{
						if (skinnedMeshRenderer.sharedMaterials != null)
						{
							foreach (var material in skinnedMeshRenderer.sharedMaterials)
							{
								if (material != null)
								{
									materials.AddUnique(material);
								}
							}
						}

						if (skinnedMeshRenderer.sharedMesh != null)
						{
							meshes.AddUnique(skinnedMeshRenderer.sharedMesh);
						}
					}

					skinMeshRenderers.Add(component as SkinnedMeshRenderer);
				}
				else
				{
					UpdateUnknownTypes(componentType);
				}
			}

			// Debug.Log($"<color=yellow><b>>>> {name}: Added {components.Count} components</b></color>");
		}

		void UpdateUnknownTypes(Type componentType)
		{
			if (componentType == GetType()) return;

			var typeName = componentType.ToString().Replace("UnityEngine.", "");
			if (typeName == "Transform") return;

			if (!unknownComponentTypes.Contains(typeName))
			{
				Debug.Log($"<color=orange><b>>>> {name}: Found unknown component type: {typeName}</b></color>");
				unknownComponentTypes.Add(typeName);
			}
		}
	}
}