using Sirenix.OdinInspector;
using System.Linq;
using Zenject;


namespace Modules.Client.EditorTools.Internal
{
    public class ModulesContext : SceneContext
    {
        [InfoBox("No need to manage this manually, child modules are fetched when you Save All with <color=cyan><b>ctrl_alt_s</b></color>")]
        [BoxGroup("Modules Context"), PropertyOrder(-1)]
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