using UnityEngine;


namespace Modules.HexTiles.Internal.DataObjects
{
    public struct Face
    {
        public Vector3[] Vertices { get; private set; }
        public int[] Triangles { get; private set; }
        public Vector2[] Uvs { get; private set; }
        public Color[] VertexColors { get; set; }

        public Face(Vector3[] vertices, int[] triangles, Vector2[] uvs, Color[] vertexColors)
        {
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
            VertexColors = vertexColors;
        }
    }
}