using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

using Modules.Client.HexTiles.Internal.Schema;


namespace Modules.Client.HexTiles.Internal.Behaviour
{
	public static class HexMeshGenerator
	{
		public static GameObject CreateTile(TileMeshPresetSo preset, Material material, Gradient heightGradient,
			float height, Transform parent, Vector3 position, string name)
		{
			var loops = new List<Face>(preset.EdgeLoops.Count * 6);
			foreach (var loop in preset.EdgeLoops)
			{
				loops.AddRange(CreateLoop(loop, heightGradient, height, preset.IsFlatTop));
			}

			var newTile = MakeTile(parent, name, position);
			newTile.filter.mesh = CombineFaces(loops);
			newTile.rend.sharedMaterial = material;
			return newTile.rend.gameObject;
		}

		static IEnumerable<Face> CreateLoop(EdgeLoop edgeLoop, Gradient heightGradient, float height, bool isFlatTop)
			=> CreateFaces(edgeLoop, heightGradient, height, isFlatTop).ToList();

		static IEnumerable<Face> CreateFaces(EdgeLoop edgeLoop, Gradient heightGradient, float height, bool isFlatTop)
		{
			var color = heightGradient.Evaluate(height);
			for (var point = 0; point < 6; point++)
			{
				yield return CreateLoop(edgeLoop, color, point, isFlatTop);
			}
		}

		static Face CreateLoop(EdgeLoop edgeLoop, Color color, int point, bool isFlatTop)
		{
			var vertices = new Vector3[4];
			vertices[0] = GetPoint(edgeLoop.InnerRadius, edgeLoop.InnerHeight, point, isFlatTop);
			vertices[1] = GetPoint(edgeLoop.InnerRadius, edgeLoop.InnerHeight, point < 5 ? point + 1 : 0, isFlatTop);
			vertices[2] = GetPoint(edgeLoop.OuterRadius, edgeLoop.OuterHeight, point < 5 ? point + 1 : 0, isFlatTop);
			vertices[3] = GetPoint(edgeLoop.OuterRadius, edgeLoop.OuterHeight, point, isFlatTop);

			var triangles = new[] { 0, 1, 2, 2, 3, 0 };
			var uvs = new[] { Vector2.zero, Vector2.right, Vector2.one, Vector2.up };

			if (edgeLoop.Reverse) Array.Reverse(vertices);

			var vertexColors = new[] { color, color, color, color };
			return new Face(vertices, triangles, uvs, vertexColors);
		}

		static Vector3 GetPoint(float size, float newHeight, int index, bool isFlatTop)
		{
			var degrees = isFlatTop ? 60 * index : 60 * index - 30;
			var radians = Mathf.PI / 180 * degrees;
			return new Vector3(size * Mathf.Cos(radians), newHeight, size * Mathf.Sin(radians));
		}

		static Mesh CombineFaces(List<Face> inputFaces)
		{
			var vertexCount = inputFaces.Count * 4;
			var vertices = new Vector3[vertexCount];
			var triangles = new int[inputFaces.Count * 6];
			var colors = new Color[vertexCount];
			var uvs = new Vector2[vertexCount];

			for (int i = 0, vertexIndex = 0, triangleIndex = 0;
			     i < inputFaces.Count;
			     i++, vertexIndex += 4, triangleIndex += 6)
			{
				inputFaces[i].Vertices.CopyTo(vertices, vertexIndex);
				inputFaces[i].VertexColors.CopyTo(colors, vertexIndex);
				inputFaces[i].Uvs.CopyTo(uvs, vertexIndex);

				var adjustedTriangles = inputFaces[i].Triangles.Select(t => t + vertexIndex).ToArray();
				adjustedTriangles.CopyTo(triangles, triangleIndex);
			}

			var result = new Mesh
			{
				name = "Hex",
				vertices = vertices,
				triangles = triangles,
				uv = uvs,
				colors = colors
			};

			result.RecalculateNormals();
			return result;
		}

		static (MeshRenderer rend, MeshFilter filter) MakeTile(Transform parent, string name, Vector3 position)
		{
			var newTile = new GameObject(name);
			newTile.transform.position = position;
			newTile.transform.SetParent(parent);
			var meshRenderer = newTile.AddComponent<MeshRenderer>();
			var meshFilter = newTile.AddComponent<MeshFilter>();
			return (meshRenderer, meshFilter);
		}
	}
}