using UnityEngine;
using System;


namespace Modules.MouseInput.External
{
	public interface IMouseInput
	{
		IObservable<(MouseState, Vector3)> LmbState { get; }
		IObservable<(MouseState, Vector3)> RmbState { get; }
		IObservable<float> WheelState { get; }
	}
}