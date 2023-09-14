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
		[Range(0.1f, 10f), SerializeField] float zoomMultiplier = 8f;
		[Range(0.1f, 10f), SerializeField] float speedMultiplier = 1.1f;

		Plane xzPlane = new(Vector3.up, Vector3.zero);
		Vector3 mouseDownPannerPosition;
		Vector3 mouseDownPos;


		void Start()
		{
			mouseInput.LmbState
				.TakeUntilDestroy(this)
				.Subscribe(OnLmb);

			mouseInput.RmbState
				.TakeUntilDestroy(this)
				.Subscribe(OnRmb);

			mouseInput.WheelState
				.TakeUntilDestroy(this)
				.Subscribe(OnMouseWheel);
		}

		void OnMouseWheel(float delta)
			=> cam.transform.Translate(
				delta * zoomMultiplier * Vector3.forward, Space.Self);

		void OnLmb((MouseState state, Vector3 mousePos) tuple)
		{
			var ray = cam.ScreenPointToRay(tuple.mousePos);

			if (!xzPlane.Raycast(ray, out var enter)) return;

			var hitPoint = ray.GetPoint(enter);
			if (tuple.state == MouseState.Click)
			{
				lookAt.position = hitPoint;
				return;
			}

			if (tuple.state == MouseState.Down)
			{
				mouseDownPos = cam.ScreenToViewportPoint(tuple.mousePos);
				mouseDownPannerPosition = panner.position;
				return;
			}

			var speed = Vector3.Distance(hitPoint, cam.transform.position) * speedMultiplier;
			var currentMousePos = cam.ScreenToViewportPoint(tuple.mousePos);
			var delta = currentMousePos - mouseDownPos;
			var newPos = mouseDownPannerPosition;
			newPos.x += delta.x * speed;
			newPos.y = panner.position.y;
			newPos.z += delta.y * speed;
			panner.position = newPos;
		}

		void OnRmb((MouseState state, Vector3 mousePos) tuple)
		{
			Debug.Log($"<color=green><b>>>> {tuple.state}</b></color>");
		}
	}
}