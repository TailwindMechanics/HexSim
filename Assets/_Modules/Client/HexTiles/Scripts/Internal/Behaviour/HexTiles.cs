using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.HexTiles.External.Schema;
using Modules.Client.HexTiles.Internal.Schema;
using Modules.Shared.HexMap.External.Schema;
using Modules.Shared.ServerApi.External;
using Modules.Client.Utilities.External;


namespace Modules.Client.HexTiles.Internal.Behaviour
{
    public class HexTiles : MonoBehaviour
    {
        [Inject] IServerApi server;

        [SerializeField] Material tilesMaterial;
        [InlineEditor, SerializeField] TileMeshPresetSo preset;

        readonly ISubject<Unit> onComplete = new Subject<Unit>();
        readonly List<MeshFilter> spawnedMeshFilters = new();
        HexGrid grid;


        void Start()
        {
            server.ServerTickStart
                .TakeUntilDestroy(this)
                .Subscribe(state =>
                {
                    transform.DestroyAllChildren();
                    grid = new HexGrid(state.Radius);
                    SpawnTiles(state);
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

            var combinedMesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            combinedMesh.CombineMeshes(combine);
            combinedMesh.Optimize();
            combinedMesh.name = "CombinedMap";

            var combinedObj = new GameObject("CombinedMap");
            var meshFilter = combinedObj.AddComponent<MeshFilter>();
            var rend = combinedObj.AddComponent<MeshRenderer>();
            meshFilter.mesh = combinedMesh;
            rend.material = tilesMaterial;

            transform.DestroyAllChildren();
            spawnedMeshFilters.Clear();

            combinedObj.transform.SetParent(transform);
        }

        void SpawnTiles(GameState state)
        {
            var offset = new Vector2Int(state.NoiseOffsetX, state.NoiseOffsetY);
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(1));
            var seed = state.Seed.ToSeedFloat();
            var scale = state.NoiseScale;
            var amp = state.Amplitude;
            var batchSize = state.Radius / 2;
            var cellBatches = Batch(grid.Cells, batchSize);

            cellBatches.ToObservable()
                .Zip(interval, (cellBatch, _) => cellBatch)
                .Finally(() => onComplete.OnNext(Unit.Default))
                .Subscribe(cellBatch =>
                {
                    cellBatch.ForEach(call => SpawnTile(call, seed, scale, amp, offset, state.HeightColorMap));
                });

            grid.OuterCells.ToObservable()
                .Zip(interval, (cell, _) => cell)
                .Finally(() => onComplete.OnNext(Unit.Default))
                .Subscribe(cell =>
                {
                    SpawnTile(cell, seed, scale, amp, offset, state.HeightColorMap);
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

        void SpawnTile (Hex2 cell, float seed, float scale, float amp, Vector2Int offset, HeightColorMapVo heightColorMap)
        {
            var height = cell.PerlinHeight(seed, scale, amp, offset);
            var color = heightColorMap.GetColorForHeight(height)
                        ?? (height > 0f ? heightColorMap.GetHighest()
                            : heightColorMap.GetLowest());

            var spawnPos = cell.ToVector3();
            spawnPos.y = height;
            var tile = HexMeshGenerator.CreateTile(preset, tilesMaterial, color, transform, spawnPos, $"Tile_{cell}");
            var newScale = tile.transform.localScale;
            newScale.y = height;
            tile.transform.localScale = newScale;
            spawnedMeshFilters.Add(tile.GetComponent<MeshFilter>());
        }
    }
}