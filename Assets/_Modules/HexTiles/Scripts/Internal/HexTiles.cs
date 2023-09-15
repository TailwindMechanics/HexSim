using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.HexTiles.External.DataObjects;
using Modules.HexMap.External.DataObjects;
using Modules.SpawnerService.External;
using Modules.TickServer.External;
using Modules.Utilities.External;
using Modules.HexMap.External;


namespace Modules.HexTiles.Internal
{
    public class HexTiles : MonoBehaviour
    {
        [Inject] ISpawnerService spawnService;
        [Inject] ITickServer tickServer;

        [SerializeField] TileSettingsSo settings;

        readonly List<GameObject> spawnedTiles = new();
        HexGrid grid;


        void Start()
            => tickServer.TickStart
                .TakeUntilDestroy(this)
                .Subscribe(tuple =>
                {
                    transform.DestroyAllChildren();
                    grid = new HexGrid(tuple.settings.GridRadius);
                    SpawnTiles(tuple.settings.Seed.ToSeed());
                });

        void SpawnTiles(float seed)
        {
            const float scale = 0.2f;
            const int offsetX = 1000;
            const int offsetY = 2000;
            const int batchSize = 10;

            var interval = Observable.Interval(TimeSpan.FromMilliseconds(10));
            var cellBatches = Batch(grid.Cells, batchSize);

            cellBatches.ToObservable()
                .Zip(interval, (cellBatch, _) => cellBatch)
                .Subscribe(cellBatch =>
                {
                    foreach (var cell in cellBatch)
                    {
                        var height = Mathf.PerlinNoise(seed + offsetX + cell.ne * scale, seed + offsetY + cell.se * scale);
                        height = Mathf.Clamp01(height);
                        SpawnTile(cell, height);
                    }
                });

            grid.OuterCells.ToObservable()
                .Zip(interval, (cell, _) => cell)
                .Subscribe(cell =>
                {
                    SpawnTile(cell, 0);
                });
        }

        IEnumerable<List<Hex2>> Batch(List<Hex2> source, int batchSize)
        {
            var batches = new List<List<Hex2>>();
            for (var i = 0; i < source.Count; i += batchSize)
            {
                batches.Add(source.GetRange(i, Math.Min(batchSize, source.Count - i)));
            }
            return batches;
        }

        void SpawnTile (Hex2 cell, float height)
        {
            var prefab = GetTilePrefab(height);
            if (prefab == null) return;

            spawnedTiles.Add(spawnService.Spawn(prefab, transform, cell.ToVector3(), $"Tile_{cell}"));
        }

        GameObject GetTilePrefab (float height)
        {
            foreach (var tile in settings.Vo.Tiles)
            {
                if (height >= tile.HeightRange.x && height <= tile.HeightRange.y)
                {
                    return tile.Prefab;
                }
            }

            return null;
        }
    }
}