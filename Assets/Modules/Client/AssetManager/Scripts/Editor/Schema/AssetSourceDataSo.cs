using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEditor.Animations;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	public class AssetSourceDataSo : ScriptableObject
	{
		[Header("# Source Info")]
		[UsedImplicitly, SerializeField] string moduleName;
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

		 public void CloneDependencies(GameObject prefab, string module, string authorName, string pack, string original)
        {
            assetPrefab = prefab;
            moduleName = module;
            author = authorName.Trim();
            packName = pack.Trim();
            originalName = original.Trim();

            var modulePath = AssetDatabase.GetAssetPath(this).Replace($"{name}.asset", "");

            foreach (var component in prefab.GetComponentsInChildren<Component>(true))
            {
                switch (component)
                {
                    case Animator animator:
                        CloneAnimator(animator, modulePath);
                        break;
                    case MeshFilter meshFilter:
                        CloneMeshFilter(meshFilter, modulePath);
                        break;
                    case MeshRenderer meshRenderer:
                        CloneMeshRenderer(meshRenderer, modulePath);
                        break;
                    case SkinnedMeshRenderer skinnedMeshRenderer:
                        CloneSkinnedMeshRenderer(skinnedMeshRenderer, modulePath);
                        break;
                    default:
                        AddToUnknownComponents(component.GetType());
                        break;
                }
            }
        }

        T CloneAsset<T>(T original, string modulePath, string extensionWithDot) where T : Object
        {
            if (original == null) return null;

            var newAssetPath = $"{modulePath}/{original.name}{extensionWithDot}";
            if (AssetDatabase.LoadAssetAtPath<T>(newAssetPath) is { } existingAsset)
            {
                return existingAsset;
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

        void CloneAnimator(Animator animator, string modulePath)
        {
            var clonedController = CloneAsset(
                animator.runtimeAnimatorController as AnimatorController,
                modulePath,
                ".controller"
            );
            var clonedAvatar = CloneAsset(
                animator.avatar,
                modulePath,
                ".asset"
            );

            animator.runtimeAnimatorController = clonedController;
            animator.avatar = clonedAvatar;

            animators.Add(new AnimatorDependencies(animator, clonedController, clonedAvatar));
        }

        void CloneMeshFilter(MeshFilter meshFilter, string modulePath)
        {
            var clonedMesh = CloneAsset(
                meshFilter.sharedMesh,
                modulePath,
                ".asset"
            );

            meshFilter.sharedMesh = clonedMesh;

            meshFilters.Add(new MeshFilterDependencies(meshFilter, clonedMesh));
        }

        void CloneMeshRenderer(MeshRenderer meshRenderer, string modulePath)
        {
            var clonedMaterialsList = new List<Material>();
            var materialDependenciesList = new List<MaterialDependencies>();

            foreach (var mat in meshRenderer.sharedMaterials)
            {
                var clonedMaterial = CloneAsset(mat, modulePath, ".mat");
                var maps = GetAllMaps(clonedMaterial, modulePath);
                clonedMaterialsList.Add(clonedMaterial);
                materialDependenciesList.Add(new MaterialDependencies(clonedMaterial, maps));
            }

            meshRenderer.sharedMaterials = clonedMaterialsList.ToArray();
            meshRenderers.Add(new MeshRendererDependencies(meshRenderer, materialDependenciesList.ToArray()));
        }

        void CloneSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer, string modulePath)
        {
            var clonedMesh = CloneAsset(
                skinnedMeshRenderer.sharedMesh,
                modulePath,
                ".asset"
            );

            var clonedMaterialsList = new List<Material>();
            var materialDependenciesList = new List<MaterialDependencies>();

            foreach (var mat in skinnedMeshRenderer.sharedMaterials)
            {
                var clonedMaterial = CloneAsset(mat, modulePath, ".mat");
                var maps = GetAllMaps(clonedMaterial, modulePath);
                clonedMaterialsList.Add(clonedMaterial);
                materialDependenciesList.Add(new MaterialDependencies(clonedMaterial, maps));
            }

            skinnedMeshRenderer.sharedMesh = clonedMesh;
            skinnedMeshRenderer.sharedMaterials = clonedMaterialsList.ToArray();

            skinMeshRenderers.Add(new SkinnedMeshRendererDependencies(skinnedMeshRenderer, clonedMesh, materialDependenciesList.ToArray()));
        }

        Texture[] GetAllMaps(Material material, string modulePath)
        {
            var maps = new List<Texture>();

            CloneAndAssignTexture(material, "_BaseMap", modulePath, maps);
            CloneAndAssignTexture(material, "_MetallicGlossMap", modulePath, maps);
            CloneAndAssignTexture(material, "_MainTex", modulePath, maps);
            CloneAndAssignTexture(material, "_BumpMap", modulePath, maps);
            CloneAndAssignTexture(material, "_OcclusionMap", modulePath, maps);
            CloneAndAssignTexture(material, "_EmissionMap", modulePath, maps);

            return maps.ToArray();
        }

        void CloneAndAssignTexture(Material material, string propertyName, string modulePath, List<Texture> maps)
        {
            Texture originalTexture = GetTexture(material, propertyName);
            if (originalTexture != null)
            {
                Texture clonedTexture = CloneAsset(originalTexture, modulePath, ".png");
                material.SetTexture(propertyName, clonedTexture); // Reassign the cloned texture
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
            => !material.HasProperty(propertyName) ? null
                : material.GetTexture(propertyName);
    }
}