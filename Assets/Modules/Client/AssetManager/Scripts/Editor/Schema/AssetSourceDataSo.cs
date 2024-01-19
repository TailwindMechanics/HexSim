using System.Collections.Generic;
using UnityEditor.Animations;
using System.Linq;
using UnityEngine;
using System;

namespace Modules.Client.AssetManager.Editor.Schema
{
	public class AssetSourceDataSo : ScriptableObject
	{
		[Header("# Source Info")]
		[SerializeField] string author;
		[SerializeField] string packName;
		[SerializeField] string originalName;

		[Header("# Dependencies")]
		public GameObject assetPrefab;
		public List<AnimatorDependencies> animators = new();
		public List<MeshFilterDependencies> meshFilters = new();
		public List<MeshRendererDependencies> meshRenderers = new();
		public List<SkinnedMeshRendererDependencies> skinMeshRenderers = new();
		[SerializeField] List<string> unknownComponentTypes = new();

		public void PopulateDependencies(GameObject prefab, string assetAuthor, string assetPackName, string assetOriginalName)
		{
			assetPrefab = prefab;
			author = assetAuthor.Trim();
			packName = assetPackName.Trim();
			originalName = assetOriginalName.Trim();

			animators.Clear();
			meshFilters.Clear();
			meshRenderers.Clear();
			skinMeshRenderers.Clear();
			unknownComponentTypes.Clear();

			var components = assetPrefab.GetComponentsInChildren<Component>(true);
			foreach (var component in components)
			{
				var componentType = component.GetType();
				if (componentType == typeof(Animator))
				{
					var animator = component as Animator;
					if (animator != null)
					{
						var dependencies = new AnimatorDependencies(
							animator,
							animator.runtimeAnimatorController as AnimatorController,
							animator.avatar
						);

						animators.Add(dependencies);
					}
				}
				else if (componentType == typeof(MeshFilter))
				{
					var meshFilter = component as MeshFilter;
					if (meshFilter != null)
					{
						var dependencies = new MeshFilterDependencies(
							meshFilter,
							meshFilter.sharedMesh
						);

						meshFilters.Add(dependencies);
					}
				}
				else if (componentType == typeof(MeshRenderer))
				{
					var meshRenderer = component as MeshRenderer;
					if (meshRenderer != null)
					{
						var dependencies = new MeshRendererDependencies(
							meshRenderer,
							meshRenderer.sharedMaterials.Where(material => material != null).ToArray()
						);

						meshRenderers.Add(dependencies);
					}
				}
				else if (componentType == typeof(SkinnedMeshRenderer))
				{
					var skinnedMeshRenderer = component as SkinnedMeshRenderer;
					if (skinnedMeshRenderer != null)
					{
						var dependencies = new SkinnedMeshRendererDependencies(
							skinnedMeshRenderer,
							skinnedMeshRenderer.sharedMesh,
							skinnedMeshRenderer.sharedMaterials.Where(material => material != null).ToArray()
						);

						skinMeshRenderers.Add(dependencies);
					}
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