﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterAnimations.Domain.Components;
using Sources.BoundedContexts.CharacterAnimations.Presentation.Views;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.TakeDamages.Domain.Events;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageAnimationSystem : IEcsRunSystem
    {
        private EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<CharacterTag, CharacterAnimationComponent, TakeDamageEvent>> _filter;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                CharacterAnimation animation = _filter.Pools.Inc2.Get(entity).Animation;

                animation.PlayHurt();
            }
        }
    }
}