using Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Implementation;
using Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Interfaces;
using Sources.BoundedContexts.Hearths.Infrastructure.Services.Implementation;
using Sources.BoundedContexts.Hearths.Infrastructure.Services.Interfaces;
using Sources.BoundedContexts.Hearths.Presentation.Views.Implementation;
using Sources.BoundedContexts.PlayerWallets.Infrastructure.Factories.Controllers;
using Sources.BoundedContexts.PlayerWallets.Infrastructure.Factories.Views;
using Sources.Frameworks.GameServices.ObjectPools.Implementation;
using Sources.Frameworks.GameServices.ObjectPools.Interfaces.Generic;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameLogicInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerWalletPresenterFactory>().AsSingle();
            Container.Bind<PlayerWalletViewFactory>().AsSingle();

            Container.Bind<IObjectPool<HearthView>>().To<ObjectPool<HearthView>>().AsSingle();
            Container.Bind<IHeartSpawnService>().To<HeartSpawnService>().AsSingle();
            Container.Bind<IHeartViewFactory>().To<HeartViewFactory>().AsSingle();
        }
    }
}