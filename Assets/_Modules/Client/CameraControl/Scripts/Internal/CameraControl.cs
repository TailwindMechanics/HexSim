using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

using Modules.Shared.GameStateRepo.External.Schema;
using Modules.Client.CameraControl.Internal.Schema;
using Modules.Client.MouseInput.External.Schema;
using Modules.Client.MouseInput.External;
using Modules.Client.Utilities.External;
using Modules.Shared.ServerApi.External;


namespace Modules.Client.CameraControl.Internal
{
	public class CameraControl : MonoBehaviour
	{
		[Inject] IMouseInput mouseInput;
		[Inject] IServerApi server;

		[SerializeField] Transform lookAt;
		[SerializeField] Transform panner;
		[SerializeField] Camera cam;
		[InlineEditor, SerializeField]
		CameraSettingsSo settings;

		Quaternion initialPannerRotation;
		Vector3 initialPannerPosition;
		float zoomAmount = .5f;
		Transform camTransform;
		Vector3 lmbDelta;
		Vector3 rmbDelta;
		float zoomDelta;
		int gridRadius;


		void Start()
			=> server.ServerTickStart
				.TakeUntilDestroy(this)
				.Subscribe(Init);

		void Init (GameState state)
		{
			gridRadius = state.Radius;
			camTransform = cam.transform;

			mouseInput.LmbViewportState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state == MouseState.Down)
				.Subscribe(_ => initialPannerPosition = panner.localPosition);

			mouseInput.RmbViewportState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state == MouseState.Down)
				.Subscribe(_ => initialPannerRotation = panner.rotation);

			mouseInput.LmbViewportState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state is MouseState.Down or MouseState.Held)
				.Select(tuple => tuple.delta)
				.Select(delta => new Vector3(delta.x, panner.position.y, delta.y))
				.Subscribe(delta => lmbDelta = delta);

			mouseInput.RmbViewportState
				.TakeUntilDestroy(this)
				.Where(tuple => tuple.state is MouseState.Down or MouseState.Held)
				.Select(tuple => tuple.delta)
				.Subscribe(delta => rmbDelta = delta);

			mouseInput.WheelDelta
				.TakeUntilDestroy(this)
				.Subscribe(delta => zoomDelta = Mathf.Clamp01(zoomDelta - delta * settings.Vo.ZoomSpeed));

			Observable.EveryLateUpdate()
				.TakeUntilDestroy(this)
				.Subscribe(_ =>
				{
					UpdatePan(lmbDelta * settings.Vo.PanSpeed);
					UpdateOrbit(rmbDelta.x * settings.Vo.OrbitSpeed);
					zoomAmount = Mathf.Lerp(zoomAmount, zoomDelta, settings.Vo.BrakeSpeed * Time.deltaTime);
					UpdateZoom(zoomAmount, settings.Vo.ZoomCurve.Evaluate(zoomAmount));
					camTransform.SmoothLookAt(lookAt.position, settings.Vo.LookAtSpeed);
				});
		}

		void UpdatePan(Vector3 delta)
			=> panner.localPosition = Vector3.Lerp(
				panner.localPosition,
				initialPannerPosition + panner.rotation * new Vector3(delta.x, 0, delta.z),
				settings.Vo.BrakeSpeed * Time.deltaTime
			);
		void UpdateOrbit(float delta)
			=> panner.rotation = Quaternion.Slerp(
				panner.rotation,
				Quaternion.AngleAxis(delta, Vector3.up) * initialPannerRotation,
				settings.Vo.BrakeSpeed * Time.deltaTime
			);
		void UpdateZoom(float delta, float curved)
			=> camTransform.localPosition = Vector3.Lerp(
				camTransform.localPosition,
				new Vector3(
					0,
					settings.Vo.ZoomMin.y + delta * (settings.Vo.ZoomOffset.y + gridRadius),
					settings.Vo.ZoomMin.z + settings.Vo.ZoomOffset.z * curved
				),
				settings.Vo.BrakeSpeed * Time.deltaTime
			);
	}
}