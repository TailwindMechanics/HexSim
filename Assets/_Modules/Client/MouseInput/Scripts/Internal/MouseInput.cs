using UnityEngine;
using Zenject;
using System;

using Modules.Client.MouseInput.External.Schema;
using Modules.Client.MouseInput.Internal.Utils;
using Modules.Client.MouseInput.External;


namespace Modules.Client.MouseInput.Internal
{
	public class MouseInput : MonoInstaller, IMouseInput
	{
		public IObservable<(MouseState state, Vector3 delta, Vector3 pos)> LmbViewportState
			=> lmbViewportState ??= MouseStateObservable.Create(0);
		public IObservable<(MouseState state, Vector3 delta, Vector3 pos)> RmbViewportState
			=> rmbViewportState ??= MouseStateObservable.Create(1);
		public IObservable<float> WheelDelta
			=> wheelDelta ??= MouseWheelObservable.Create();

		IObservable<(MouseState state, Vector3 delta, Vector3 pos)> lmbViewportState;
		IObservable<(MouseState state, Vector3 delta, Vector3 pos)> rmbViewportState;
		IObservable<float> wheelDelta;

		public override void InstallBindings()
			=> Container.Bind<IMouseInput>().FromInstance(this).AsSingle();
	}
}