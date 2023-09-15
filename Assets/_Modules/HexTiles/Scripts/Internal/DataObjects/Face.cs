using System.Collections.Generic;
using UnityEngine;


namespace Modules.HexTiles.Internal.DataObjects
{
    public struct Face
    {
        public List<Vector3> Vertices { get; private set; }
        public List<int> Triangles { get; private set; }
        public List<Vector2> Uvs { get; private set; }
        public List<Color> VertexColors { get; set; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Color> vertexColors)
        {
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
            VertexColors = vertexColors;
        }
    }
}