﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterTakeDamages.Domain.Componets;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyTakeDamages.Infrastructure.Systems
{
    public class EnemyTakeDamageParticleSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<EnemyTag, TakeDamageParticleComponent, TakeDamageEvent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                TakeDamageParticleComponent takeDamageParticleComponent = _filter.Pools.Inc2.Get(entity);
                
                if (takeDamageParticleComponent.ParticleSystem.isPlaying)
                    continue;
                
                takeDamageParticleComponent.ParticleSystem.Play();
                Debug.Log($"Play particle system");
            }
        }
    }
}