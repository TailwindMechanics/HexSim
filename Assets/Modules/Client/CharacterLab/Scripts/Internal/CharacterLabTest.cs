using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;

using Modules.Client.AssetManager.External.Schema;


namespace Modules.Client.CharacterLab.Internal
{
    public class CharacterLabTest : MonoBehaviour
    {
        [SerializeField] AssetReferenceGameObject characterAssetRef;
        [SerializeField] HashedTextureRef textureRef;
        [SerializeField] Transform parent;

        [SerializeField]
        List<GameObject> spawnedCharacters = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SpawnCharacter();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                DespawnCharacter();
            }
        }

        async void SpawnCharacter()
        {
            Debug.Log("CharacterLabTest: SpawnCharacter() called.");

            var spawnedCharacter = await characterAssetRef.InstantiateAsync(parent).Task;
            spawnedCharacters.Add(spawnedCharacter);
        }

        void DespawnCharacter()
        {
            Debug.Log("CharacterLabTest: DespawnCharacter() called.");

            var despawn = spawnedCharacters[^1];
            spawnedCharacters.Remove(despawn);
            characterAssetRef.ReleaseInstance(despawn);
        }
    }
}