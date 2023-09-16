using UnityEngine;


namespace Modules.Utilities.External
{
	public static class CameraExtensions
	{
		public static Vector3? ScreenToPlane(this Camera camera, Vector3 screenPoint, Plane plane, float height = 0f)
		{
			var ray = camera.ScreenPointToRay(screenPoint);
			plane.distance = -height;
			if (plane.Raycast(ray, out var enter))
			{
				return ray.GetPoint(enter);
			}

			return null;
		}
	}
}