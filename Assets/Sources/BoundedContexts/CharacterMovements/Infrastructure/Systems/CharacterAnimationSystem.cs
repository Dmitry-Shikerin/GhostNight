using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterMovements.Presentation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterAnimationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, CharacterAnimationComponent>> _filter = default;

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

                if (_eventsBus.HasEventSingleton(out InputEvent inputEvent) == false)
                    return;

                if (inputEvent.Direction != Vector2.zero)
                    animation.PlayWalk();
                else
                    animation.PlayIdle();
            }
        }
    }
}