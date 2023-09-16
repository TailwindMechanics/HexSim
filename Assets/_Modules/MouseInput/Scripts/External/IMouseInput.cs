using UnityEngine;
using System;


namespace Modules.MouseInput.External
{
	public interface IMouseInput
	{
		IObservable<(MouseState state, Vector3 pos)> LmbState { get; }
		IObservable<(MouseState state, Vector3 pos)> RmbState { get; }
		IObservable<float> WheelState { get; }
	}
}