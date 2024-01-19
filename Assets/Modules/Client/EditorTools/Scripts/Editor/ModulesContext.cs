using System.Linq;
using UnityEngine;
using Zenject;

namespace Modules.Client.EditorTools.Editor
{
    public class ModulesContext : SceneContext
    {
        [Header("No need to manage this manually, child modules are fetched when you Save All with <color=cyan><b>ctrl_alt_s</b></color>")]
        public int modulesCount;

        public int SetInstallers ()
        {
            var modules = GetComponentsInChildren<MonoInstaller>(true).ToList();
            modulesCount = modules.Count;
            Installers = modules;
            return modulesCount;
        }
    }
}