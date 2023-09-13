using Sirenix.OdinInspector;
using System.Linq;
using Zenject;


namespace Modules.EditorTools.Internal
{
    public class ModulesContext : SceneContext
    {
        [BoxGroup("Modules Context"), PropertyOrder(-1), InfoBox("No need to manage this manually, child modules are fetched when you Save All via ctrl_alt_s.")]
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