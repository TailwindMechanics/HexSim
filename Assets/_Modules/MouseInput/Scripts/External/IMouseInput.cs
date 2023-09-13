using UnityEngine;
using System;


namespace Modules.MouseInput.External
{
	public interface IMouseInput
	{
		IObservable<Vector3> MouseDown { get; }
	}
}