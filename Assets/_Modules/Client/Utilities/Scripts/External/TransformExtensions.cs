﻿using UnityEngine;


namespace Modules.Client.Utilities.External
{
	public static class TransformExtensions
	{
		public static void ZeroOutLocalPositionAndRotation(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		public static void DestroyAllChildren(this Transform transform)
		{
			if (Application.isPlaying)
			{
				foreach (Transform child in transform) Object.Destroy(child.gameObject);
			}
			else if (Application.isEditor)
			{
				while (transform.childCount > 0)
				{
					var child = transform.GetChild(0);
					Object.DestroyImmediate(child.gameObject);
				}
			}
		}

		public static void SmoothLookAt(this Transform transform, Vector3 targetPosition, float speed)
		{
			var targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
		}
	}
}