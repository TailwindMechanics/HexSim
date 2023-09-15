using UnityEngine;

namespace Modules.SpawnerService.External
{
    public interface ISpawnerService
    {
        public T Spawn<T>(T prefab, Transform parent = null, Vector3 position = default, string name = null, Quaternion rotation = default) where T : Object;
    }
}