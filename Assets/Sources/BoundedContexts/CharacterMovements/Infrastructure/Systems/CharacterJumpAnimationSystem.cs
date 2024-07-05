using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterAnimations.Presentation.Views;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Jumps.Domain.Components;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterJumpAnimationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag,
                CharacterAnimationComponent,
                DirectionComponent,
                JumpComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                CharacterAnimation animation = _filter.Pools.Inc2.Get(entity).Animation;

                animation.PlayJump();
            }
        }
    }
}