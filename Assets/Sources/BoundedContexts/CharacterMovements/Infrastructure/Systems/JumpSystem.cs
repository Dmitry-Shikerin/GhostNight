using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class JumpSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, JumpComponent, MovementComponent, CharacterAnimationComponent>> _filter = default;

        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref MovementComponent movementComponent = ref _filter.Pools.Inc3.Get(entity);
                ref JumpComponent jumpComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterAnimationComponent animationComponent = ref _filter.Pools.Inc4.Get(entity);
                
                if (_eventsBus.HasEventSingleton<JumpEvent>() == false)
                    return;
                
                ref JumpEvent jumpBody = ref _eventsBus.GetEventBodySingleton<JumpEvent>();

                if (jumpBody.IsJumped)
                {
                    jumpComponent.CurrentDalay += Time.deltaTime;
                    Vector3 direction = Time.deltaTime * new Vector3(0, jumpComponent.JumpForce, 0);
                    movementComponent.CharacterController.Move(direction);

                    if (jumpComponent.CurrentDalay >= jumpComponent.Delay)
                    {
                        jumpBody.IsJumped = false;
                        movementComponent.IsLockMovement = false;
                        jumpComponent.CurrentDalay = 0;
                        _eventsBus.DestroyEventSingleton<JumpEvent>();
                    }
                    
                    return;
                }
                
                movementComponent.IsLockMovement = true;
                jumpBody.IsJumped = true;
            }
        }
    }
}