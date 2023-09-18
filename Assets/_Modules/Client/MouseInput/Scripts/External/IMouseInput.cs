using UnityEngine;
using System;

using Modules.Client.MouseInput.External.Schema;


namespace Modules.Client.MouseInput.External
{
	public interface IMouseInput
	{
		IObservable<(MouseState state, Vector3 delta, Vector3 pos)> LmbViewportState { get; }
		IObservable<(MouseState state, Vector3 delta, Vector3 pos)> RmbViewportState { get; }
		IObservable<float> WheelDelta { get; }
	}
}