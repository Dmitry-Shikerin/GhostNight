﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Hammers.Domain.Events;
using Sources.BoundedContexts.TakeDamages.Domain.Components;

namespace Sources.BoundedContexts.Hammers.Infrastructure.Systems
{
    public class HammerAttackBlockTakeDamageSystem : IEcsRunSystem
    {
        private const float Delay = 2f;
        private readonly EcsFilterInject<Inc<CharacterTag, StartHummerAttackEvent>> _startAttackFilter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _startAttackFilter.Value)
            {
                ref BlockTakeDamageComponent blockTakeDamageComponent = 
                    ref systems.GetWorld().GetPool<BlockTakeDamageComponent>().Add(entity);
                
                blockTakeDamageComponent.Duration = Delay;
                blockTakeDamageComponent.CurrentTime = 0f;
            }
        }
    }
}