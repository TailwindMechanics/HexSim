using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

using Modules.HexTiles.Internal.DataObjects;


namespace Modules.HexTiles.Internal.Behaviour
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexRenderer : MonoBehaviour
    {
        [SerializeField]
        Material material;

        [Range(0f, 2f), OnValueChanged(nameof(OnChange)), SerializeField]
        float inner = 0.5f;
        [Range(0f, 2f), OnValueChanged(nameof(OnChange)), SerializeField]
        float outer = 1f;
        [Range(0f, 2f), OnValueChanged(nameof(OnChange)), SerializeField]
        float height = 1f;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;


        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start() => DrawMesh();
        void OnChange () => DrawMesh();

        void DrawMesh()
        {
            if (!Application.isPlaying) return;

            var faces = DrawFaces(inner, outer, height);
            meshFilter.mesh = CombineFaces(faces);
            meshRenderer.material = material;
        }

        List<Face> DrawFaces (float innerSize, float outerSize, float newHeight)
        {
            var result = new List<Face>();
            var halfHeight = newHeight / 2f;

            var topFaces = DrawFaces(innerSize, outerSize, halfHeight, halfHeight);
            var bottomFaces = DrawFaces(innerSize, outerSize, -halfHeight, -halfHeight, true);
            var outerFaces = DrawFaces(outerSize, outerSize, halfHeight, -halfHeight, true);
            var innerFaces = DrawFaces(innerSize, innerSize, halfHeight, -halfHeight);

            result.AddRange(topFaces);
            result.AddRange(bottomFaces);
            result.AddRange(outerFaces);
            result.AddRange(innerFaces);

            return result;
        }

        IEnumerable<Face> DrawFaces (float innerSize, float outerSize, float heightA, float heightB, bool reverse = false)
        {
            var result = new List<Face>();
            for (var point = 0; point < 6; point++)
            {
                result.Add(CreateFace(innerSize, outerSize, heightA, heightB, point, reverse));
            }

            return result;
        }

        Face CreateFace (float innerRadius, float outerRadius, float heightA, float heightB, int point, bool reverse = false)
        {
            var pointA = GetPoint(innerRadius, heightB, point);
            var pointB = GetPoint(innerRadius, heightB, point < 5 ? point + 1 : 0);
            var pointC = GetPoint(outerRadius, heightA, point < 5 ? point + 1 : 0);
            var pointD = GetPoint(outerRadius, heightA, point);

            var vertices = new List<Vector3> { pointA, pointB, pointC, pointD };
            var triangles = new List<int> { 0, 1, 2, 2, 3, 0 };
            var uvs = new List<Vector2> { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };
            if (reverse) vertices.Reverse();

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