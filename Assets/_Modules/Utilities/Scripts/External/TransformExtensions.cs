using UnityEngine;


namespace Modules.Utilities.External
{
	public static class TransformExtensions
	{
		public static void ZeroOutLocalPositionAndRotation(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}