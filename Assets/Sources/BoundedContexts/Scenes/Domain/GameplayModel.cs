﻿using Sources.BoundedContexts.PlayerWallets.Domain.Models;
using Sources.Frameworks.GameServices.Volumes.Domain.Models.Implementation;

namespace Sources.BoundedContexts.Scenes.Domain
{
    public class GameplayModel
    {
        public GameplayModel(
            // Upgrade characterHealthUpgrade,
            // Upgrade characterAttackUpgrade,
            // Upgrade nukeAbilityUpgrade,
            // Upgrade flamethrowerAbilityUpgrade,
            // Bunker bunker,
            // EnemySpawner enemySpawner,
            // CharacterSpawnAbility characterSpawnAbility,
            // NukeAbility nukeAbility,
            // FlamethrowerAbility flamethrowerAbility,
            // KillEnemyCounter killEnemyCounter,
            PlayerWallet playerWallet,
            Volume volume)
        {
            // CharacterHealthUpgrade = characterHealthUpgrade;
            // CharacterAttackUpgrade = characterAttackUpgrade;
            // NukeAbilityUpgrade = nukeAbilityUpgrade;
            // FlamethrowerAbilityUpgrade = flamethrowerAbilityUpgrade;
            // Bunker = bunker;
            // EnemySpawner = enemySpawner;
            // CharacterSpawnAbility = characterSpawnAbility;
            // NukeAbility = nukeAbility;
            // FlamethrowerAbility = flamethrowerAbility;
            // KillEnemyCounter = killEnemyCounter;
            PlayerWallet = playerWallet;
            Volume = volume;
        }

        // public Upgrade CharacterHealthUpgrade { get; }
        // public Upgrade CharacterAttackUpgrade { get; }
        // public Upgrade NukeAbilityUpgrade { get; }
        // public Upgrade FlamethrowerAbilityUpgrade { get; }
        // public Bunker Bunker { get; }
        // public EnemySpawner EnemySpawner { get; }
        // public CharacterSpawnAbility CharacterSpawnAbility { get; }
        // public NukeAbility NukeAbility { get; }
        // public FlamethrowerAbility FlamethrowerAbility { get; }
        // public KillEnemyCounter KillEnemyCounter { get; }
        public PlayerWallet PlayerWallet { get; }
        public Volume Volume { get; }
    }
}