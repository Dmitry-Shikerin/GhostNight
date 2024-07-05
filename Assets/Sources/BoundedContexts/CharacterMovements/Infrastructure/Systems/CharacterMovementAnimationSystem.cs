using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterAnimations.Presentation.Views;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Jumps.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterMovementAnimationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag,
                CharacterAnimationComponent,
                DirectionComponent>,
            Exc<JumpComponent,
                BlockMovementComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                CharacterAnimation animation = _filter.Pools.Inc2.Get(entity).Animation;
                DirectionComponent directionComponent = _filter.Pools.Inc3.Get(entity);
                
                if (directionComponent.Direction != Vector2.zero)
                    animation.PlayWalk();
                else
                    animation.PlayIdle();
            }
        }
    }
}