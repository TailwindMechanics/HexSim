using UnityEngine;
using System;
using UniRx;

using Modules.Client.MouseInput.External.Schema;


namespace Modules.Client.MouseInput.Internal.Utils
{
	public class MouseStateObservable
	{
		readonly ISubject<(MouseState, Vector3, Vector3)> mouseStateSubject = new Subject<(MouseState, Vector3, Vector3)>();
		Vector3 initialMousePosition = Vector3.zero;
		MouseState currentState = MouseState.None;
		const float clickDistanceThreshold = 0.1f;
		const float clickTimeThreshold = 0.2f;
		readonly int buttonIndex;
		float timeMouseDown;

		Camera Cam => camera ??= Camera.main;
		Camera camera;


		public static IObservable<(MouseState state, Vector3 delta, Vector3 pos)> Create(int buttonIndex, IObservable<Unit> stop)
			=> new MouseStateObservable(buttonIndex, stop).Get();

		MouseStateObservable(int buttonIndex, IObservable<Unit> stop)
		{
			this.buttonIndex = buttonIndex;
			Observable.EveryUpdate()
				.TakeUntil(stop)
				.Subscribe(_ => UpdateMouseState());
		}

		IObservable<(MouseState, Vector3, Vector3)> Get()
			=> mouseStateSubject;

		void UpdateMouseState()
		{
			if (Input.GetMouseButtonDown(buttonIndex)) currentState = MouseState.Down;
			else if (Input.GetMouseButtonUp(buttonIndex)) currentState = MouseState.Up;
			else if (currentState == MouseState.None) return;

			var mousePosition = Input.mousePosition;
			var delta = Vector3.zero;
			if (currentState == MouseState.Down)
			{
				timeMouseDown = Time.time;
				initialMousePosition = mousePosition;
				EmitMouseState(MouseState.Down, delta);
				currentState = MouseState.Held;
				return;
			}

			var curr = Cam.ScreenToViewportPoint(mousePosition);
			var initial = Cam.ScreenToViewportPoint(initialMousePosition);
			delta = curr - initial;

			if (currentState == MouseState.Up)
			{
				currentState = DidNotBreachClickThreshold()
					? MouseState.Click
					: MouseState.Up;
				EmitMouseState(currentState, delta);
				currentState = MouseState.None;
				return;
			}

			EmitMouseState(currentState, delta);
		}

		void EmitMouseState(MouseState state, Vector3 delta)
			=> mouseStateSubject.OnNext((state, delta, Cam.ScreenToViewportPoint(Input.mousePosition)));
		bool DidNotBreachClickThreshold()
			=> !BreachedClickTimeThreshold() && !BreachedClickDistanceThreshold(Input.mousePosition);
		bool BreachedClickTimeThreshold()
			=> Time.time - timeMouseDown >= clickTimeThreshold;
		bool BreachedClickDistanceThreshold(Vector3 mousePosition)
			=> Vector3.Distance(initialMousePosition, mousePosition)
			   >= Screen.width * clickDistanceThreshold;
	}
}