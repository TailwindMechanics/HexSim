using UnityEngine;
using System;
using UniRx;

using Modules.Client.MouseInput.External.Schema;


namespace Modules.Client.MouseInput.Internal.Utils
{
	public class MouseStateObservable
	{
		readonly ISubject<(MouseState,  Vector3)> mouseStateSubject = new Subject<(MouseState,  Vector3)>();
		Vector3 initialMousePosition = Vector3.zero;
		MouseState currentState = MouseState.None;
		const float clickDistanceThreshold = 0.1f;
		const float clickTimeThreshold = 0.2f;
		readonly int buttonIndex;
		float timeMouseDown;


		public static IObservable<(MouseState,  Vector3)> Create(int buttonIndex)
			=> new MouseStateObservable(buttonIndex).Get();

		MouseStateObservable(int buttonIndex)
		{
			this.buttonIndex = buttonIndex;
			Observable.EveryUpdate()
				.Subscribe(_ => UpdateMouseState());
		}

		IObservable<(MouseState,  Vector3)> Get()
			=> mouseStateSubject;

		void UpdateMouseState()
		{
			if (Input.GetMouseButtonDown(buttonIndex)) currentState = MouseState.Down;
			else if (Input.GetMouseButtonUp(buttonIndex)) currentState = MouseState.Up;
			else if (currentState == MouseState.None) return;

			var mousePosition = Input.mousePosition;
			if (currentState == MouseState.Down)
			{
				timeMouseDown = Time.time;
				initialMousePosition = mousePosition;
				EmitMouseState(MouseState.Down);
				currentState = MouseState.Held;
				return;
			}

			if (currentState == MouseState.Up)
			{
				currentState = DidNotBreachClickThreshold()
					? MouseState.Click
					: MouseState.Up;
				EmitMouseState(currentState);
				currentState = MouseState.None;
				return;
			}

			mouseStateSubject.OnNext((currentState, mousePosition));
		}

		void EmitMouseState(MouseState state)
			=> mouseStateSubject.OnNext((state, Input.mousePosition));
		bool DidNotBreachClickThreshold()
			=> !BreachedClickTimeThreshold() && !BreachedClickDistanceThreshold(Input.mousePosition);
		bool BreachedClickTimeThreshold()
			=> Time.time - timeMouseDown >= clickTimeThreshold;
		bool BreachedClickDistanceThreshold(Vector3 mousePosition)
			=> Vector3.Distance(initialMousePosition, mousePosition)
			   >= Screen.width * clickDistanceThreshold;
	}
}