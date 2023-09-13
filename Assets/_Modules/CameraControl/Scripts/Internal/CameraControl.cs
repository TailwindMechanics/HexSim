using UnityEngine;
using Zenject;
using UniRx;

using Modules.CameraControl.External;
using Modules.MouseInput.External;


namespace Modules.CameraControl.Internal
{
    public class CameraControl : MonoInstaller, ICameraControl
    {
        [Inject] IMouseInput mouseInput;

        public override void InstallBindings()
            => Container.Bind<ICameraControl>().FromInstance(this).AsSingle();

        public override void Start()
            => mouseInput.MouseDown
                .TakeUntilDestroy(this)
                .Subscribe(OnMouseWentDown);

        void OnMouseWentDown(Vector3 mousePosition)
        {
            Debug.Log($"<color=yellow><b>>>> mousePosition: {mousePosition}</b></color>");
        }
    }
}