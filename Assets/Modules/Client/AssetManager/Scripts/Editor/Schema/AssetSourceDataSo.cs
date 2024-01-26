using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEditor.Animations;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	public class AssetSourceDataSo : ScriptableObject
	{
		[Header("# Source Info")] [UsedImplicitly, SerializeField]
		string moduleName;

		[UsedImplicitly, SerializeField] string author;
		[UsedImplicitly, SerializeField] string packName;
		[UsedImplicitly, SerializeField] string originalName;

		[Header("# Dependencies"), UsedImplicitly]
		public GameObject assetPrefab;

		public List<AnimatorDependencies> animators = new();
		public List<MeshFilterDependencies> meshFilters = new();
		public List<MeshRendererDependencies> meshRenderers = new();
		public List<SkinnedMeshRendererDependencies> skinMeshRenderers = new();
		[SerializeField] List<string> unknownComponentTypes = new();

		public void CloneDependencies(GameObject prefab, string moduleAssetsPath, string module, string authorName, string pack, string original)
		{
			assetPrefab = prefab;
			moduleName = module;
			author = authorName.Trim();
			packName = pack.Trim();
			originalName = original.Trim();

			var assetPath = AssetDatabase.GetAssetPath(this).Replace($"{name}.asset", "");

			foreach (var component in prefab.GetComponentsInChildren<Component>(true))
			{
				switch (component)
				{
					case Animator animator:
						CloneAnimator(animator, moduleAssetsPath, assetPath);
						break;
					case MeshFilter meshFilter:
						CloneMeshFilter(meshFilter, moduleAssetsPath, assetPath);
						break;
					case MeshRenderer meshRenderer:
						CloneMeshRenderer(meshRenderer, moduleAssetsPath, assetPath);
						break;
					case SkinnedMeshRenderer skinnedMeshRenderer:
						CloneSkinnedMeshRenderer(skinnedMeshRenderer, moduleAssetsPath, assetPath);
						break;
					default:
						AddToUnknownComponents(component.GetType());
						break;
				}
			}
		}

		// T CloneAsset<T>(T original, string modulePath, string extensionWithDot) where T : Object
		// {
		// 	if (original == null) return null;
		//
		// 	var newAssetPath = $"{modulePath}/{original.name}{extensionWithDot}";
		// 	if (AssetDatabase.LoadAssetAtPath<T>(newAssetPath) is { } existingAsset)
		// 	{
		// 		if (newAssetPath.Contains("Shared")) return existingAsset;
		//
		// 		// Move the asset to the Shared folder
		// 		var sharedFolderPath = $"{modulePath}/Shared";
		// 		if (!AssetDatabase.IsValidFolder(sharedFolderPath))
		// 		{
		// 			AssetDatabase.CreateFolder(modulePath, "Shared");
		// 		}
		//
		// 		var existingAssetPath = AssetDatabase.GetAssetPath(existingAsset);
		// 		var newSharedAssetPath = $"{sharedFolderPath}/{original.name}{extensionWithDot}";
		// 		AssetDatabase.MoveAsset(existingAssetPath, newSharedAssetPath);
		// 		return AssetDatabase.LoadAssetAtPath<T>(newSharedAssetPath);
		// 	}
		//
		// 	if (original is Texture texture)
		// 	{
		// 		SetTextureReadable(AssetDatabase.GetAssetPath(texture));
		// 		AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(texture), newAssetPath);
		// 		var clonedTexture = AssetDatabase.LoadAssetAtPath<T>(newAssetPath);
		// 		AddressableUtil.AddToAddressablesGroup(clonedTexture, moduleName);
		// 		return clonedTexture;
		// 	}
		//
		// 	var clonedAsset = Instantiate(original);
		// 	AssetDatabase.CreateAsset(clonedAsset, newAssetPath);
		// 	AddressableUtil.AddToAddressablesGroup(clonedAsset, moduleName);
		// 	return clonedAsset;
		// }

		Object FindAsset<T>(string assetName, string folder)
		{
			return FindAssetInFolder<T>(assetName, folder);
		}

		Object FindAssetInFolder<T>(string assetName, string folder)
		{
			// Search in the current folder
			var assetGuids = AssetDatabase.FindAssets(assetName, new[] { folder });
			foreach (var guid in assetGuids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
				if (asset != null) return asset;
			}

			// Recursively search in subfolders
			var subFolders = AssetDatabase.GetSubFolders(folder);
			foreach (var subFolder in subFolders)
			{
				var foundAsset = FindAssetInFolder<T>(subFolder, assetName);
				if (foundAsset != null) return foundAsset;
			}

			return null; // Asset not found
		}

		T CloneAsset<T>(T original, string moduleAssetsPath, string assetPath, string extensionWithDot) where T : Object
		{
			if (original == null) return null;

			var newAssetPath = $"{assetPath}{original.name}{extensionWithDot}";
			var foundAsset = FindAsset<T>(original.name, moduleAssetsPath);
			if (foundAsset != null)
			{
				var foundAssetPath = AssetDatabase.GetAssetPath(foundAsset);
				if (foundAssetPath == newAssetPath) return foundAsset as T;
				if (foundAssetPath.Contains("Shared")) return foundAsset as T;

				Debug.Log($"<color=yellow><b>>>> foundAssetPath: {foundAssetPath}, newAssetPath: {newAssetPath}</b></color>");

				var sharedFolderPath = $"{moduleAssetsPath}/Shared";
				if (!AssetDatabase.IsValidFolder(sharedFolderPath))
				{
					Debug.Log($"<color=yellow><b>>>> creating sharedFolderPath: {sharedFolderPath}</b></color>");
					AssetDatabase.CreateFolder(moduleAssetsPath, "Shared");
				}
				
				Debug.Log($"<color=yellow><b>>>> moving foundAsset: {foundAsset}</b></color>");
				var existingAssetPath = AssetDatabase.GetAssetPath(foundAsset);
				var newSharedAssetPath = $"{sharedFolderPath}/{original.name}{extensionWithDot}";
				AssetDatabase.MoveAsset(existingAssetPath, newSharedAssetPath);

				return foundAsset as T;
			}

			if (original is Texture texture)
			{
				SetTextureReadable(AssetDatabase.GetAssetPath(texture));
				AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(texture), newAssetPath);
				var clonedTexture = AssetDatabase.LoadAssetAtPath<T>(newAssetPath);
				AddressableUtil.AddToAddressablesGroup(clonedTexture, moduleName);
				return clonedTexture;
			}

			var clonedAsset = Instantiate(original);
			AssetDatabase.CreateAsset(clonedAsset, newAssetPath);
			AddressableUtil.AddToAddressablesGroup(clonedAsset, moduleName);
			return clonedAsset;
		}

		void AddToUnknownComponents(Type type)
		{
			var typeName = type.ToString().Replace("UnityEngine.", "");
			if (typeName == "Transform") return;
			if (typeName == "Modules.Client.AssetManager.External.AssetSourceData") return;

			if (!unknownComponentTypes.Contains(typeName))
			{
				unknownComponentTypes.Add(typeName);
			}
		}

		void CloneAnimator(Animator animator, string moduleAssetsPath, string assetPath)
		{
			var clonedController = CloneAsset(
				animator.runtimeAnimatorController as AnimatorController,
				moduleAssetsPath,
				assetPath,
				".controller"
			);
			var clonedAvatar = CloneAsset(
				animator.avatar,
				moduleAssetsPath,
				assetPath,
				".asset"
			);

			animator.runtimeAnimatorController = clonedController;
			animator.avatar = clonedAvatar;

			animators.Add(new AnimatorDependencies(animator, clonedController, clonedAvatar));
		}

		void CloneMeshFilter(MeshFilter meshFilter, string moduleAssetsPath, string assetPath)
		{
			var clonedMesh = CloneAsset(
				meshFilter.sharedMesh,
				moduleAssetsPath,
				assetPath,
				".asset"
			);

			meshFilter.sharedMesh = clonedMesh;

			meshFilters.Add(new MeshFilterDependencies(meshFilter, clonedMesh));
		}

		void CloneMeshRenderer(MeshRenderer meshRenderer, string moduleAssetsPath, string assetPath)
		{
			var clonedMaterialsList = new List<Material>();
			var materialDependenciesList = new List<MaterialDependencies>();

			foreach (var mat in meshRenderer.sharedMaterials)
			{
				var clonedMaterial = CloneAsset(
					mat,
					moduleAssetsPath,
					assetPath,
					".mat"
				);
				var maps = GetAllMaps(clonedMaterial, moduleAssetsPath, assetPath);
				clonedMaterialsList.Add(clonedMaterial);
				materialDependenciesList.Add(new MaterialDependencies(clonedMaterial, maps));
			}

			meshRenderer.sharedMaterials = clonedMaterialsList.ToArray();
			meshRenderers.Add(new MeshRendererDependencies(meshRenderer, materialDependenciesList.ToArray()));
		}

		void CloneSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer, string moduleAssetsPath, string assetPath)
		{
			var clonedMesh = CloneAsset(
				skinnedMeshRenderer.sharedMesh,
				moduleAssetsPath,
				assetPath,
				".asset"
			);

			var clonedMaterialsList = new List<Material>();
			var materialDependenciesList = new List<MaterialDependencies>();

			foreach (var mat in skinnedMeshRenderer.sharedMaterials)
			{
				var clonedMaterial = CloneAsset(
					mat,
					moduleAssetsPath,
					assetPath,
					".mat"
				);
				var maps = GetAllMaps(clonedMaterial, moduleAssetsPath, assetPath);
				clonedMaterialsList.Add(clonedMaterial);
				materialDependenciesList.Add(new MaterialDependencies(clonedMaterial, maps));
			}

			skinnedMeshRenderer.sharedMesh = clonedMesh;
			skinnedMeshRenderer.sharedMaterials = clonedMaterialsList.ToArray();

			skinMeshRenderers.Add(new SkinnedMeshRendererDependencies(skinnedMeshRenderer, clonedMesh,
				materialDependenciesList.ToArray()));
		}

		Texture[] GetAllMaps(Material material, string modulesAssetPath, string assetPath)
		{
			var maps = new List<Texture>();

			CloneAndAssignTexture(material, "_BaseMap", modulesAssetPath, assetPath, maps);
			CloneAndAssignTexture(material, "_MetallicGlossMap", modulesAssetPath, assetPath, maps);
			CloneAndAssignTexture(material, "_MainTex", modulesAssetPath, assetPath, maps);
			CloneAndAssignTexture(material, "_BumpMap", modulesAssetPath, assetPath, maps);
			CloneAndAssignTexture(material, "_OcclusionMap", modulesAssetPath, assetPath, maps);
			CloneAndAssignTexture(material, "_EmissionMap", modulesAssetPath, assetPath, maps);

			return maps.ToArray();
		}

		void CloneAndAssignTexture(Material material, string propertyName, string moduleAssetsPath, string assetPath, List<Texture> maps)
		{
			var originalTexture = GetTexture(material, propertyName);
			if (originalTexture != null)
			{
				var clonedTexture = CloneAsset(
					originalTexture,
					moduleAssetsPath,
					assetPath,
					".png"
				);
				material.SetTexture(propertyName, clonedTexture);
				maps.Add(clonedTexture);
			}
		}


		void SetTextureReadable(string assetPath)
		{
			var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (textureImporter == null) return;

			textureImporter.isReadable = true;
			textureImporter.SaveAndReimport();
		}

		Texture GetTexture(Material material, string propertyName)
			=> !material.HasProperty(propertyName)
				? null
				: material.GetTexture(propertyName);
	}
}