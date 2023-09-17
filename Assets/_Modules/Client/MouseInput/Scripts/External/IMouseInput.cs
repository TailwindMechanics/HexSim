using UnityEngine;
using System;

using Modules.Client.MouseInput.External.Schema;


namespace Modules.Client.MouseInput.External
{
	public interface IMouseInput
	{
		IObservable<(MouseState state, Vector3 pos)> LmbState { get; }
		IObservable<(MouseState state, Vector3 pos)> RmbState { get; }
		IObservable<float> WheelState { get; }
	}
}