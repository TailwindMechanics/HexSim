using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Modules.HexTiles.Internal.DataObjects;


namespace Modules.HexTiles.Internal.Behaviour
{
    [Serializable]
    public class EdgeLoop
    {
        public float InnerRadius => innerRadius;
        public float InnerHeight => innerHeight;
        public float OuterRadius => outerRadius;
        public float OuterHeight => outerHeight;
        public bool Reverse => reverse;

        [FoldoutGroup("$GroupName"), SerializeField]
        string label = "Untitled";

        [FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
        float innerRadius = .9f;
        [FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
        float outerRadius = .9f;

        [FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
        float innerHeight = 1f;
        [FoldoutGroup("$GroupName"), Range(0f, 1f), SerializeField]
        float outerHeight = 1f;
        [FoldoutGroup("$GroupName"), SerializeField]
        bool reverse;

        [UsedImplicitly]
        string GroupName => string.IsNullOrEmpty(label) ? "Untitled" : label;
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexRenderer : MonoBehaviour
    {
        [SerializeField]
        Material material;
        [SerializeField]
        bool update;
        [SerializeField]
        List<EdgeLoop> edgeLoops = new();

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;


        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        void Update()
        {
            if (update) DrawMesh2();
        }

        void DrawMesh2 ()
        {
            var loops = new List<Face>();
            edgeLoops.ForEach(loop =>
            {
                var faces = CreateLoop(loop);
                loops.AddRange(faces);
            });

            meshFilter.mesh = CombineFaces(loops);
            meshRenderer.material = material;
        }

        List<Face> CreateLoop (EdgeLoop edgeLoop)
        {
            var result = new List<Face>();
            var faces = CreateFaces(edgeLoop);
            result.AddRange(faces);

            return result;
        }

        IEnumerable<Face> CreateFaces (EdgeLoop edgeLoop)
        {
            var result = new List<Face>();
            for (var point = 0; point < 6; point++)
            {
                result.Add(CreateFace(edgeLoop, point));
            }

            return result;
        }

        Face CreateFace (EdgeLoop edgeLoop, int point)
        {
            var pointA = GetPoint(edgeLoop.InnerRadius, edgeLoop.InnerHeight, point);
            var pointB = GetPoint(edgeLoop.InnerRadius, edgeLoop.InnerHeight, point < 5 ? point + 1 : 0);
            var pointC = GetPoint(edgeLoop.OuterRadius, edgeLoop.OuterHeight, point < 5 ? point + 1 : 0);
            var pointD = GetPoint(edgeLoop.OuterRadius, edgeLoop.OuterHeight, point);

            var vertices = new List<Vector3> { pointA, pointB, pointC, pointD };
            var triangles = new List<int> { 0, 1, 2, 2, 3, 0 };
            var uvs = new List<Vector2> { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };
            if (edgeLoop.Reverse) vertices.Reverse();

            return new Face(vertices, triangles, uvs);
        }

        Vector3 GetPoint (float size, float newHeight, int index)
        {
            var degrees = 60 * index;
            var radians = Mathf.PI / 180 * degrees;
            return new Vector3(size * Mathf.Cos(radians), newHeight, size * Mathf.Sin(radians));
        }

        Mesh CombineFaces (IReadOnlyList<Face> inputFaces)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            for (var i = 0; i < inputFaces.Count; i++)
            {
                vertices.AddRange(inputFaces[i].Vertices);
                uvs.AddRange(inputFaces[i].Uvs);
                triangles.AddRange(inputFaces[i].Triangles.Select(triangle => triangle + 4 * i));
            }

            var result = new Mesh
            {
                name = "Hex",
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };

            result.RecalculateNormals();
            return result;
        }
    }
}