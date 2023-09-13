using UnityEngine;
using Zenject;
using System;
using UniRx;

using Modules.MouseInput.External;


namespace Modules.MouseInput.Internal
{
	public class MouseInput : MonoInstaller, IMouseInput
	{
		public IObservable<Vector3> MouseDown => mouseDown;
		readonly ISubject<Vector3> mouseDown = new Subject<Vector3>();

		public override void InstallBindings()
			=> Container.Bind<IMouseInput>().FromInstance(this).AsSingle();

		public override void Start()
			=> Observable.EveryUpdate()
				.TakeUntilDestroy(this)
				.Select(_ => Input.GetMouseButtonDown(0))
				.Where(onMouseDown => onMouseDown)
				.Select(_ => Input.mousePosition)
				.Subscribe(MouseWentDown);

		void MouseWentDown(Vector3 mousePosition)
			=> mouseDown.OnNext(mousePosition);
	}
}