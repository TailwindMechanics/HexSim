using Zenject;

using Modules.TickServer.External;


namespace Modules.TickServer.Internal
{
    public class TickServer : MonoInstaller, ITickServer
    {
        public override void InstallBindings()
            => Container.Bind<ITickServer>().FromInstance(this).AsSingle();
    }
}