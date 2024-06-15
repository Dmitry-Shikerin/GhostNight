using Sources.BoundedContexts.PlayerWallets.Infrastructure.Factories.Controllers;
using Sources.BoundedContexts.PlayerWallets.Infrastructure.Factories.Views;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameLogicInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerWalletPresenterFactory>().AsSingle();
            Container.Bind<PlayerWalletViewFactory>().AsSingle();
        }
    }
}