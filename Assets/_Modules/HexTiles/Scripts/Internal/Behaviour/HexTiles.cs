using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.HexTiles.Internal.DataObjects;
using Modules.HexMap.External.DataObjects;
using Modules.TickServer.External;
using Modules.Utilities.External;
using Modules.HexMap.External;


namespace Modules.HexTiles.Internal.Behaviour
{
    public class HexTiles : MonoBehaviour
    {
        [Inject] ITickServer tickServer;

        [FoldoutGroup("Spawn a tile"), Button(ButtonSizes.Medium)]
        void SpawnATile ()
        {
            transform.DestroyAllChildren();
            SpawnTile(new Hex2(0, 0), .5f);
        }

        [SerializeField] Vector2 minMaxHeight = new(0, 5);
        [SerializeField] Gradient heightGradient;
        [SerializeField] Material tilesMaterial;
        [SerializeField] TileSettingsSo settings;

        readonly ISubject<Unit> onComplete = new Subject<Unit>();
        readonly List<MeshFilter> spawnedMeshFilters = new();
        HexGrid grid;


        void Start()
        {
            tickServer.TickStart
                .TakeUntilDestroy(this)
                .Subscribe(tuple =>
                {
                    transform.DestroyAllChildren();
                    grid = new HexGrid(tuple.settings.GridRadius);
                    SpawnTiles(tuple.settings.Seed.ToSeed());
                });

            onComplete.Skip(1)
                .TakeUntilDestroy(this)
                .Subscribe(_ => CombineAndWeldMeshes());
        }

        void CombineAndWeldMeshes()
        {
            var combine = new CombineInstance[spawnedMeshFilters.Count];

            for (var i = 0; i < spawnedMeshFilters.Count; i++)
            {
                combine[i].mesh = spawnedMeshFilters[i].sharedMesh;
                combine[i].transform = spawnedMeshFilters[i].transform.localToWorldMatrix;
            }

            var combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine);
            combinedMesh.Optimize();

            combinedMesh.name = "CombinedMap";

            var combinedObj = new GameObject("CombinedMap");
            var meshFilter = combinedObj.AddComponent<MeshFilter>();
            meshFilter.mesh = combinedMesh;
            var rend = combinedObj.AddComponent<MeshRenderer>();
            rend.material = tilesMaterial;

            transform.DestroyAllChildren();
            spawnedMeshFilters.Clear();
        }

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
                .Finally(() => onComplete.OnNext(Unit.Default))
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
                .Finally(() => onComplete.OnNext(Unit.Default))
                .Subscribe(cell =>
                {
                    var height = Mathf.PerlinNoise(seed + offsetX + cell.ne * scale, seed + offsetY + cell.se * scale);
                    height = Mathf.Clamp01(height);
                    SpawnTile(cell, height);
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
            var preset = GetTilePreset(height);
            if (preset == null) return;

            var tile = HexMeshGenerator.CreateTile(preset, tilesMaterial, heightGradient, height, transform, cell.ToVector3(), $"Tile_{cell}");
            var newScale = tile.transform.localScale;
            newScale.y = minMaxHeight.x + height * minMaxHeight.y;
            tile.transform.localScale = newScale;
            spawnedMeshFilters.Add(tile.GetComponent<MeshFilter>());
        }

        TileMeshPresetSo GetTilePreset (float height)
            => settings.Tiles.FirstOrDefault(tile => WithinHeightRange(height, tile));
        bool WithinHeightRange (float height, TileMeshPresetSo preset)
            => height >= preset.MinHeight && height <= preset.MaxHeight;
    }
}