using UnityEngine;
using Zenject;

using Modules.SpawnerService.External;


namespace Modules.SpawnerService.Internal
{
    public class Spawner : MonoInstaller, ISpawnerService
    {
        public override void InstallBindings()
            => Container.Bind<ISpawnerService>().FromInstance(this).AsSingle();
        public T Spawn<T>(T prefab, Transform parent = null, Vector3 position = default, Quaternion rotation = default) where T : Object
            => Instantiate(prefab, position, rotation, parent == null ? transform : parent);
    }
}