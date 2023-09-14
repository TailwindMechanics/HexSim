using UnityEngine;
using Zenject;
using System;

using Modules.MouseInput.Internal.Utils;
using Modules.MouseInput.External;


namespace Modules.MouseInput.Internal
{
	public class MouseInput : MonoInstaller, IMouseInput
	{
		IObservable<(MouseState, Vector3)> lmbInstance;
		IObservable<(MouseState, Vector3)> rmbInstance;
		IObservable<float> wheelInstance;

		public IObservable<(MouseState, Vector3)> LmbState
			=> lmbInstance ??= MouseStateObservable.Create(0);
		public IObservable<(MouseState, Vector3)> RmbState
			=> rmbInstance ??= MouseStateObservable.Create(1);
		public IObservable<float> WheelState
			=> wheelInstance ??= MouseWheelObservable.Create();
		public override void InstallBindings()
			=> Container.Bind<IMouseInput>().FromInstance(this).AsSingle();
	}
}