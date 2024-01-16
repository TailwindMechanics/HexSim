using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Modules.Client.CharacterLab.External
{
    [CreateAssetMenu(fileName = "ModuleAssetImporterSo", menuName = "Modules/CharacterLab/ModuleAssetImporter")]
    public class ModuleAssetImporterSo : ScriptableObject
    {
        public List<HashedMaterialRef> materials;
        public List<HashedTextureRef> textures;
        public List<HashedMeshRef> meshes;
        public List<HashedAnimatorRef> animators;
        public List<HashedMeshRendRef> meshRenderers;
        public List<HashedSkinMeshRendRef> skinMeshRenderers;
        public List<HashedMeshFilterRef> meshFilters;
        public List<HashedMeshColliderRef> meshColliders;

        [SerializeField] bool refresh;
        [SerializeField] GameObject root;
        [SerializeField] List<string> componentsTypes = new();

        void OnValidate()
        {
            if (root == null) return;
            if (!refresh) return;

            componentsTypes = root.GetComponentsInChildren<Component>(true)
                .Select(c => c.GetType().ToString().Replace("UnityEngine.", "")).Distinct().ToList();

            Debug.Log($"<color=yellow><b>>>> OnValidate {componentsTypes.Count}</b></color>");

            refresh = false;
        }
    }
}