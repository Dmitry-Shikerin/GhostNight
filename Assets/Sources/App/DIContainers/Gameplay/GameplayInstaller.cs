using Sources.App.Ecs;
using Sources.App.Ecs.Controllers.Implementation;
using Sources.App.Ecs.Controllers.Interfaces;
using Sources.BoundedContexts.Huds.Presentations;
using Sources.BoundedContexts.RootGameObjects.Presentation;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Controllers.Implementation;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Controllers.Interfaces;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Domain.Implementation;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Views.Implementation;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Views.Interfaces;
using Sources.Frameworks.GameServices.AddressablesInfr.AssetServices.Implementation;
using Sources.Frameworks.GameServices.AddressablesInfr.AssetServices.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayHud _gameplayHud;
        [SerializeField] private RootGameObject rootGameObject;
        
        public override void InstallBindings()
        {
            Container.Bind<GameplayHud>().FromInstance(_gameplayHud).AsSingle();
            Container.Bind<RootGameObject>().FromInstance(rootGameObject).AsSingle();
            
            Container.Bind<ISceneFactory>().To<GameplaySceneFactory>().AsSingle();
            Container.Bind<GameplayModelsCreatorService>().AsSingle();
            Container.Bind<GameplayModelsLoaderService>().AsSingle();
            Container.Bind<ISceneViewFactory>().To<GameplaySceneViewFactory>().AsSingle();
            Container.Bind<IEcsStartUp>().To<EcsStartUp>().AsSingle();
            Container.Bind<ICompositeAssetService>().To<CompositeAssetService>().AsSingle();
        }
    }
}