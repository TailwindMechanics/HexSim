using System.Collections.Generic;
using UnityEngine;
using System;

namespace Modules.Server.NeuroNavigation.External
{
	public static class RenderPath
	{
		public static LineRenderer UseLineRenderer (LineRenderer lineRenderer, NeuroPath neuroPath, Func<Vector3, float> getHeightAtPos = null, float heightOffset = 0f)
		{
			lineRenderer.positionCount = neuroPath.Path.Count;
			for (var i = 0; i < neuroPath.Path.Count; i++)
			{
				var node = neuroPath.Path[i];
				var height = (getHeightAtPos?.Invoke(node.Pos) ?? 1f) + heightOffset;
				var pos = new Vector3(node.Pos.x, height, node.Pos.z);
				lineRenderer.SetPosition(i, pos);
			}

			return lineRenderer;
		}

		public static List<GameObject> SpawnPrimitives (NeuroPath neuroPath, Transform parent = null, Func<Vector3, float> getHeightAtPos = null, PrimitiveType primitive = PrimitiveType.Sphere)
		{
			var spawned = new List<GameObject>();
			foreach (var node in neuroPath.Path)
			{
				var marker = GameObject.CreatePrimitive(primitive);
				var height = getHeightAtPos?.Invoke(node.Pos) ?? 1f;
				var pos = new Vector3(node.Pos.x, height, node.Pos.z);
				marker.transform.position = pos;
				marker.name = node.ToString();
				marker.transform.localScale = Vector3.one * 0.2f;
				marker.transform.parent = parent;
				spawned.Add(marker);
			}

			return spawned;
		}
	}
}