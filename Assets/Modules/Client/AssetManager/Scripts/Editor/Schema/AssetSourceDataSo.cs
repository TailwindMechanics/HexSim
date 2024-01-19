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

        T CloneAsset<T>(T original, string modulePath) where T : Object
        {
            if (original == null) return null;

            var newAssetPath = $"{modulePath}/{original.name}.asset";
            if (AssetDatabase.LoadAssetAtPath<T>(newAssetPath) is { } existingAsset)
            {
                return existingAsset;
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
                modulePath
            );
            var clonedAvatar = CloneAsset(animator.avatar, modulePath);

            animator.runtimeAnimatorController = clonedController;
            animator.avatar = clonedAvatar;

            animators.Add(new AnimatorDependencies(animator, clonedController, clonedAvatar));
        }

        void CloneMeshFilter(MeshFilter meshFilter, string modulePath)
        {
            var clonedMesh = CloneAsset(meshFilter.sharedMesh, modulePath);

            meshFilter.sharedMesh = clonedMesh;

            meshFilters.Add(new MeshFilterDependencies(meshFilter, clonedMesh));
        }

        void CloneMeshRenderer(MeshRenderer meshRenderer, string modulePath)
        {
            var clonedMaterials = meshRenderer.sharedMaterials
                .Select(m => CloneAsset(m, modulePath))
                .ToArray();

            meshRenderer.sharedMaterials = clonedMaterials;

            meshRenderers.Add(new MeshRendererDependencies(meshRenderer, clonedMaterials));
        }

        void CloneSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer, string modulePath)
        {
            var clonedMesh = CloneAsset(skinnedMeshRenderer.sharedMesh, modulePath);
            var clonedMaterials = skinnedMeshRenderer.sharedMaterials
                .Select(m => CloneAsset(m, modulePath))
                .ToArray();

            skinnedMeshRenderer.sharedMesh = clonedMesh;
            skinnedMeshRenderer.sharedMaterials = clonedMaterials;

            skinMeshRenderers.Add(new SkinnedMeshRendererDependencies(skinnedMeshRenderer, clonedMesh, clonedMaterials));
        }
	}
}