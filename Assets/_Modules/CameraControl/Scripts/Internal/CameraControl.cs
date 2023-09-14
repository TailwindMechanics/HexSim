using UnityEngine;
using Zenject;
using UniRx;

using Modules.CameraControl.External;
using Modules.MouseInput.External;


namespace Modules.CameraControl.Internal
{
	public class CameraControl : MonoBehaviour, ICameraControl
	{
		[Inject] IMouseInput mouseInput;

		[SerializeField] Transform lookAt;
		[SerializeField] Transform panner;
		[SerializeField] Camera cam;
		[Range(0.1f, 10f), SerializeField] float zoomSpeed = 5f;
		[Range(0.1f, 10f), SerializeField] float panSpeed = 1.1f;
		[Range(0.1f, 10f), SerializeField] float orbitSpeed = .1f;

		Plane xzPlane = new(Vector3.up, Vector3.zero);
		Vector2Int cameraLimits = new(1, 20);
		Vector3 mouseDownPannerPosition;
		Transform camTransform;
		Vector3 lmbDownPos;
		Vector3 rmbDownPos;


		void Start()
		{
			camTransform = cam.transform;

			mouseInput.LmbState
				.TakeUntilDestroy(this)
				.Subscribe(OnLmb);

			mouseInput.RmbState
				.TakeUntilDestroy(this)
				.Subscribe(OnRmb);

			mouseInput.WheelState
				.TakeUntilDestroy(this)
				.Where(CameraDidNotBreachedLimit)
				.Subscribe(OnMouseWheel);

			Observable.EveryLateUpdate()
				.TakeUntilDestroy(this)
				.Subscribe(_ => camTransform.LookAt(lookAt));
		}

		void OnMouseWheel(float delta)
			=> camTransform.Translate(
				delta * zoomSpeed * (Vector3.forward - Vector3.up / 3), Space.Self);

		void OnLmb((MouseState state, Vector3 mousePos) tuple)
		{
			if (tuple.state == MouseState.Click) return;
			var ray = cam.ScreenPointToRay(tuple.mousePos);
			if (!xzPlane.Raycast(ray, out var enter)) return;

			if (tuple.state == MouseState.Down)
			{
				lmbDownPos = cam.ScreenToViewportPoint(tuple.mousePos);
				mouseDownPannerPosition = panner.position;
				return;
			}

			var hitPoint = ray.GetPoint(enter);
			var speed = Vector3.Distance(hitPoint, camTransform.position) * panSpeed;
			var currentMousePos = cam.ScreenToViewportPoint(tuple.mousePos);
			var delta = currentMousePos - lmbDownPos;
			var worldDelta = camTransform.TransformDirection(new Vector3(delta.x, 0, delta.y));
			var newPos = mouseDownPannerPosition - worldDelta * speed;
			newPos.y = panner.position.y;

			panner.position = newPos;
		}

		void OnRmb((MouseState state, Vector3 mousePos) tuple)
		{
			if (tuple.state == MouseState.Down)
			{
				rmbDownPos = tuple.mousePos;
			}
			else if (tuple.state == MouseState.Held)
			{
				var delta = tuple.mousePos.x - rmbDownPos.x;
				var rotationDelta = delta * orbitSpeed;
				panner.RotateAround(lookAt.position, Vector3.up, rotationDelta);
				rmbDownPos = tuple.mousePos;
			}
		}

		bool CameraDidNotBreachedLimit (float delta)
		{
			var lowerLimitBreached = delta >= 0 && camTransform.position.y < cameraLimits.x;
			var upperLimitBreached = delta < 0 && camTransform.position.y > cameraLimits.y;
			return !lowerLimitBreached && !upperLimitBreached;
		}
	}
}