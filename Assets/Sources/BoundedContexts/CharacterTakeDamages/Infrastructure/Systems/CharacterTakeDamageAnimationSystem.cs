using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterMovements.Presentation.Views;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using UnityEngine;

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

                if (_world.Value.GetPool<BlockMovementComponent>().Has(entity))
                    continue;
                
                animation.PlayHurt();
                _world.Value.GetPool<BlockMovementComponent>().Add(entity);
                Debug.Log($"PlayHurt");
            }
        }
    }
}