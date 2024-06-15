using System;
using Sources.BoundedContexts.Ids.Domain.Constant;
using Sources.BoundedContexts.PlayerWallets.Domain.Models;
using Sources.BoundedContexts.Scenes.Domain;
using Sources.BoundedContexts.Scenes.Infrastructure.Factories.Domain.Interfaces;
using Sources.Frameworks.GameServices.Volumes.Domain.Models.Implementation;
using Sources.InfrastructureInterfaces.Services.Repositories;

namespace Sources.BoundedContexts.Scenes.Infrastructure.Factories.Domain.Implementation
{
    public class GameplayModelsCreatorService : IGameplayModelsLoaderService
    {
        private readonly IEntityRepository _entityRepository;

        public GameplayModelsCreatorService(
            IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository ?? 
                                throw new ArgumentNullException(nameof(entityRepository));
        }

        public GameplayModel Load()
        {
            //PlayerWallet
            PlayerWallet playerWallet = new PlayerWallet(50, ModelId.PlayerWallet);
            _entityRepository.Add(playerWallet);
            
            //Volume
            Volume volume = new Volume(ModelId.Volume);
            _entityRepository.Add(volume);
            
            return new GameplayModel(
                // characterHealthUpgrade,
                // characterAttackUpgrade,
                // nukeAbilityUpgrade,
                // flamethrowerAbilityUpgrade,
                // bunker,
                // enemySpawner,
                // characterSpawnAbility,
                // nukeAbility,
                // flamethrowerAbility,
                // killEnemyCounter,
                playerWallet,
                volume);
        }
    }
}