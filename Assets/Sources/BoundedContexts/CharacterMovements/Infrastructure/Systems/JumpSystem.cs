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
                ref InputEvent inputEvent = ref _eventsBus.GetEventBodySingleton<InputEvent>();

                if (jumpBody.IsJumped)
                {
                    if (jumpComponent.CurrentDalay < jumpComponent.Delay)
                    {
                        jumpComponent.CurrentDalay += Time.deltaTime;
                        Vector3 direction = 
                            Time.deltaTime 
                            * new Vector3(
                                inputEvent.Direction.x + 0.2f, 
                                jumpComponent.JumpForce, 
                                inputEvent.Direction.y + 0.2f);
                        movementComponent.CharacterController.Move(direction);
                    }
                    else
                    {
                        if (movementComponent.CharacterController.isGrounded)
                        {
                            jumpComponent.CurrentDalay = 0;
                            jumpBody.IsJumped = false;
                            _eventsBus.DestroyEventSingleton<JumpEvent>();
                            movementComponent.IsLockMovement = false;
                        }
                        
                        Vector3 moveDirection = 
                            Time.deltaTime 
                            * movementComponent.Speed
                            * new Vector3(inputEvent.Direction.x, 0, inputEvent.Direction.y);
                        moveDirection.y -= movementComponent.Gravity * Time.deltaTime;
                        movementComponent.CharacterController.Move(moveDirection);
                    }
                    
                    return;
                }

                movementComponent.IsLockMovement = true;
                jumpBody.IsJumped = true;
            }
        }
    }
}