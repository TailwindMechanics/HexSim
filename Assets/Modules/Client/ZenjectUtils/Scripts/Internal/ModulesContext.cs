#if UNITY_EDITOR

using System.Linq;
using UnityEngine;
using Zenject;

using Modules.Client.EditorTools.Editor;

namespace Modules.Client.ZenjectUtils.Internal
{
    [ExecuteInEditMode, RequireComponent(typeof(SceneContext))]
    public class ModulesContext : MonoBehaviour
    {
        SceneContext SceneContext => sceneContext ??= GetComponent<SceneContext>();
        SceneContext sceneContext;


        [Header("• This auto-sets the Installers on SceneContext" +
                "\n    when 'Save All' is triggered via ctrl_alt_s." +
                "\n• No need to manually manage SceneContext")]
        public int modulesCount;

        void OnEnable()
            => EditorSave.OnSaved.AddListener(SetInstallers);
        void OnDisable()
            => EditorSave.OnSaved.RemoveListener(SetInstallers);

        void SetInstallers (string logColor)
        {
            var modules = GetComponentsInChildren<MonoInstaller>(true).ToList();
            modulesCount = modules.Count;
            SceneContext.Installers = modules;
            Debug.Log($"<color={logColor}><b>Saved {modulesCount} modules.</b></color>");
        }
    }
}

#endif