using UnityEngine;
using Zenject;

using Modules.SpawnerService.External;


namespace Modules.SpawnerService.Internal
{
    public class Spawner : MonoInstaller, ISpawnerService
    {
        public override void InstallBindings()
            => Container.Bind<ISpawnerService>().FromInstance(this).AsSingle();

        public T Spawn<T>(T prefab, Transform parent = null, Vector3 position = default, string name = null, Quaternion rotation = default) where T : Object
        {
            var result = Instantiate(prefab, position, rotation, parent == null ? transform : parent);
            result.name = name ?? result.name;
            return result;
        }
    }
}