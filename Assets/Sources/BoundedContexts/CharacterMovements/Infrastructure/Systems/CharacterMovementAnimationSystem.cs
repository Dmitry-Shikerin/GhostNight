using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterMovements.Presentation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterMovementAnimationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag,
                CharacterAnimationComponent,
                DirectionComponent>,
            Exc<JumpComponent>> _filter = default;

        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                CharacterAnimation animation = _filter.Pools.Inc2.Get(entity).Animation;

                if (_eventsBus.HasEventSingleton<JumpEvent>())
                {
                    animation.PlayJump();

                    return;
                }

                DirectionComponent directionComponent = _filter.Pools.Inc3.Get(entity);

                if (directionComponent.Direction != Vector2.zero)
                    animation.PlayWalk();
                else
                    animation.PlayIdle();
            }
        }
    }
}