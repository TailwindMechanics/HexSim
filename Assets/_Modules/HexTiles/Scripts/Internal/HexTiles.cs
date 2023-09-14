using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

using Modules.HexMap.External.DataObjects;
using Modules.SpawnerService.External;
using Modules.TickServer.External;
using Modules.HexMap.External;


namespace Modules.HexTiles.Internal
{
    public class HexTiles : MonoBehaviour
    {
        [Inject] ISpawnerService spawnService;
        [Inject] ITickServer tickServer;

        [SerializeField] GameObject tilePrefab;
        HexGrid grid;

        readonly List<GameObject> spawnedTiles = new();

        void Start()
        {
            Debug.Log($"<color=yellow><b>>>> HexTiles/Start</b></color>");
            tickServer.TickStart
                .TakeUntilDestroy(this)
                .Subscribe(tuple =>
                {
                    Debug.Log("go");
                    grid = new HexGrid(tuple.settings.GridRadius);
                    SpawnTiles();
                });
        }

        void SpawnTiles ()
        {
            grid.FlatCells.ForEach(cell =>
            {
                var spawnedTile = spawnService.Spawn(tilePrefab, transform, cell.ToVector3());
                spawnedTiles.Add(spawnedTile);
            });
        }
    }
}