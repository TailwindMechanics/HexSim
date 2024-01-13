using UnityEngine;
using System;


namespace Modules.Client.CameraControl.Internal.Schema
{
	[Serializable]
	public class CameraSettingsVo
	{
		public float PanSpeed => panSpeed;
		public float OrbitSpeed => orbitSpeed;
		public float ZoomSpeed => zoomSpeed;
		public float LookAtSpeed => lookAtSpeed;
		public float BrakeSpeed => brakeSpeed;
		public AnimationCurve ZoomCurve => zoomCurve;
		public Vector3 ZoomOffset => zoomOffset;
		public Vector3 ZoomMin => zoomMin;

		[Range(0f, 10f), SerializeField] float zoomSpeed = 1f;
		[SerializeField] Vector3 zoomOffset = new (0, 50, 10);
		[SerializeField] Vector3 zoomMin = new (0, 2, 5);
		[SerializeField] AnimationCurve zoomCurve;
		[Range(-100f, 100f), SerializeField] float panSpeed = 10f;
		[Range(-100f, 100f), SerializeField] float orbitSpeed = 10f;
		[Range(0f, 10f), SerializeField] float lookAtSpeed = 1f;
		[Range(0f, 10f), SerializeField] float brakeSpeed = 1f;
	}
}