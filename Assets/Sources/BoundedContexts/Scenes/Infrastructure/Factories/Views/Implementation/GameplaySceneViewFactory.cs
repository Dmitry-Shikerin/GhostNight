using System;
using Sources.App.Ecs.Controllers.Interfaces;
using Sources.BoundedContexts.Huds.Presentations;
using Sources.BoundedContexts.PlayerWallets.Infrastructure.Factories.Views;
using Sources.BoundedContexts.RootGameObjects.Presentation;
using Sources.BoundedContexts.Scenes.Domain;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Domain.Implementation;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Views.Interfaces;
using Sources.Frameworks.GameServices.Scenes.Domain.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.Collectors;

namespace Sources.BoundedContexts.Scenes.Infrastructure.Factories.Views.Implementation
{
    public class GameplaySceneViewFactory : ISceneViewFactory
    {
        private readonly GameplayHud _gameplayHud;
        private readonly UiCollectorFactory _uiCollectorFactory;
        private readonly RootGameObject _rootGameObject;
        private readonly IAudioService _audioService;
        private readonly GameplayModelsCreatorService _gameplayModelsCreatorService;
        private readonly GameplayModelsLoaderService _gameplayModelsLoaderService;
        private readonly PlayerWalletViewFactory _playerWalletViewFactory;
        private readonly IEcsStartUp _ecsStartUp;

        public GameplaySceneViewFactory(
            GameplayHud gameplayHud,
            UiCollectorFactory uiCollectorFactory,
            RootGameObject rootGameObject,
            IAudioService audioService,
            GameplayModelsCreatorService gameplayModelsCreatorService,
            GameplayModelsLoaderService gameplayModelsLoaderService,
            PlayerWalletViewFactory playerWalletViewFactory,
            IEcsStartUp ecsStartUp)
        {
            _gameplayHud = gameplayHud ?? throw new ArgumentNullException(nameof(gameplayHud));
            _uiCollectorFactory = uiCollectorFactory ?? throw new ArgumentNullException(nameof(uiCollectorFactory));
            _rootGameObject = rootGameObject ?? throw new ArgumentNullException(nameof(rootGameObject));
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            _gameplayModelsCreatorService = gameplayModelsCreatorService ?? 
                                            throw new ArgumentNullException(nameof(gameplayModelsCreatorService));
            _gameplayModelsLoaderService = gameplayModelsLoaderService ?? 
                                           throw new ArgumentNullException(nameof(gameplayModelsLoaderService));
            _playerWalletViewFactory = playerWalletViewFactory ??
                                       throw new ArgumentNullException(nameof(playerWalletViewFactory));
            _ecsStartUp = ecsStartUp ?? throw new ArgumentNullException(nameof(ecsStartUp));
        }

        public void Create(IScenePayload payload)
        {
            GameplayModel gameplayModel = Load(payload);
            
            //PlayerWallet
            _playerWalletViewFactory.Create(_gameplayHud.PlayerWalletView);
            
            //UiCollector
            _uiCollectorFactory.Create();

            //Volume
            _audioService.Construct(gameplayModel.Volume);
        }

        private GameplayModel Load(IScenePayload payload)
        {
            if (payload != null && payload.CanLoad)
                return _gameplayModelsLoaderService.Load();
            
            return _gameplayModelsCreatorService.Load();
        }
    }
}