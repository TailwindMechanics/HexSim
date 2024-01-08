using UnityEngine;
using System;
using UniRx;


namespace Modules.Client.MouseInput.Internal.Utils
{
	public class MouseWheelObservable
	{
		readonly ISubject<float> mouseWheelSubject = new Subject<float>();

		public static IObservable<float> Create()
			=> new MouseWheelObservable().Get();

		MouseWheelObservable()
			=> Observable.EveryUpdate()
				.Subscribe(_ => UpdateMouseWheel());

		IObservable<float> Get()
			=> mouseWheelSubject;

		void UpdateMouseWheel()
		{
			var scrollAmount = Input.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(scrollAmount) > Mathf.Epsilon)
			{
				mouseWheelSubject.OnNext(scrollAmount);
			}
		}
	}
}