using Zenject;

using Modules.Server.GameLogic.External;


namespace Modules.Server.GameLogic.Internal
{
    public class GameLogicInstaller : MonoInstaller
    {
        public override void InstallBindings()
            => Container.Bind<IGameLogic>().To<GameLogic>().AsSingle();
    }
}