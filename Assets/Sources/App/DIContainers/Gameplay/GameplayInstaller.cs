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
using UnityEngine;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayHud _gameplayHud;
        [SerializeField] private RootGameObject _rootGameObject;
        
        public override void InstallBindings()
        {
            Container.Bind<GameplayHud>().FromInstance(_gameplayHud).AsSingle();
            Container.Bind<RootGameObject>().FromInstance(_rootGameObject).AsSingle();
            
            Container.Bind<ISceneFactory>().To<GameplaySceneFactory>().AsSingle();
            Container.Bind<GameplayModelsCreatorService>().AsSingle();
            Container.Bind<GameplayModelsLoaderService>().AsSingle();
            Container.Bind<ISceneViewFactory>().To<GameplaySceneViewFactory>().AsSingle();
            Container.Bind<IEcsStartUp>().To<EcsStartUp>().AsSingle();
        }
    }
}