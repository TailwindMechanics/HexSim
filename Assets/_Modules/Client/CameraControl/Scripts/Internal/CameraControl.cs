using System;
using UnityEngine;
using Zenject;
using UniRx;

using Modules.Client.MouseInput.External.Schema;
using Modules.Client.MouseInput.External;
using Modules.Client.Utilities.External;


namespace Modules.Client.CameraControl.Internal
{
	public class CameraControl : MonoBehaviour
	{
		[Inject] IMouseInput mouseInput;

		[SerializeField] Transform lookAt;
		[SerializeField] Transform panner;
		[SerializeField] Camera cam;
		[SerializeField] Vector2 cameraHeightLimits = new(2, 120);
		[SerializeField] Vector2 cameraForwardLimits = new(-10, -.1f);
		[Range(0.1f, 10f), SerializeField] float zoomSpeed = 5f;
		[Range(0.1f, 10f), SerializeField] float panSpeed = 1.1f;
		[Range(0.1f, 10f), SerializeField] float orbitSpeed = .1f;

		readonly Plane xzPlane = new(Vector3.up, Vector3.zero);
		Vector3 mouseDownPannerPosition;
		Transform camTransform;

		MouseState rmbState;
		Vector3 rmbPosition;
		Vector3 rmbDownPos;

		MouseState lmbState;
		Vector3 lmbPosition;
		Vector3 lmbDownPos;

		float wheelDelta;
		float rmbDelta;


		void Start()
		{
			camTransform = cam.transform;

			mouseInput.LmbState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state is MouseState.Down or MouseState.Held)
				.Subscribe(tuple =>
				{
					lmbState = tuple.state;
					if (lmbState == MouseState.Down) lmbDownPos = tuple.pos;
					else lmbPosition = tuple.pos;
				});

			mouseInput.RmbState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state is MouseState.Down or MouseState.Held)
				.Subscribe(tuple =>
				{
					rmbState = tuple.state;
					if (rmbState == MouseState.Down)
					{
						rmbDownPos = tuple.pos;
					}
					else
					{
						rmbDelta = tuple.pos.x - rmbDownPos.x;
					}
				});

			mouseInput.WheelState
				.TakeUntilDestroy(this)
				.Subscribe(delta =>
				{
					wheelDelta = delta;
				});
		}

		void LateUpdate()
		{
			rmbDelta = UpdateOrbit(rmbDelta);
			camTransform.LookAt(lookAt);
		}

		void UpdatePan(Vector3 downMousePos, Vector3 mousePos)
		{
			var planePoint = cam.ScreenToPlane(mousePos, xzPlane);
			if (planePoint == null) return;

			var speed = Vector3.Distance(planePoint.Value, camTransform.position) * panSpeed * Time.deltaTime;
			var currentMousePos = cam.ScreenToViewportPoint(mousePos);
			var delta = currentMousePos - downMousePos;
			var worldDelta = camTransform.TransformDirection(new Vector3(delta.x, 0, delta.y));
			var newPos = mouseDownPannerPosition - worldDelta * speed;
			newPos.y = panner.position.y;
			panner.position = newPos;
		}

		float UpdateOrbit(float delta)
		{
			var rotationDelta = delta * orbitSpeed;
			panner.RotateAround(lookAt.position, Vector3.up, rotationDelta);
			return 0f;
		}

		float UpdateZoom(float delta)
		{
			camTransform.Translate(delta * zoomSpeed * (Vector3.forward - Vector3.up), Space.Self);
			return 0f;
			// var clampedPos = camTransform.localPosition;
			// clampedPos.y = Mathf.Clamp(clampedPos.y, cameraHeightLimits.x, cameraHeightLimits.y);
			// clampedPos.z = Mathf.Clamp(clampedPos.z, cameraForwardLimits.x, cameraForwardLimits.y);
			// camTransform.localPosition = clampedPos;
		}
	}
}