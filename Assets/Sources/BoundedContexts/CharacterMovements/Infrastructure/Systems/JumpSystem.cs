using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class JumpSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, JumpComponent, CharacterControllerComponent>> _filter = default;
        private readonly EcsWorldInject _world = default;

        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref JumpComponent jumpComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterControllerComponent controllerComponent = ref _filter.Pools.Inc3.Get(entity);

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
                        controllerComponent.CharacterController.Move(direction);
                    }
                    else
                    {
                        if (controllerComponent.CharacterController.isGrounded)
                        {
                            jumpComponent.CurrentDalay = 0;
                            jumpBody.IsJumped = false;
                            ref var movementComponent = ref _world.Value.GetPool<MovementComponent>().Add(entity);
                            movementComponent.Gravity = jumpComponent.Gravity;
                            movementComponent.Speed = jumpComponent.Speed;
                            _eventsBus.DestroyEventSingleton<JumpEvent>();
                        }
                        
                        Vector3 moveDirection = 
                            Time.deltaTime 
                            * jumpComponent.Speed
                            * new Vector3(inputEvent.Direction.x, 0, inputEvent.Direction.y);
                        moveDirection.y -= jumpComponent.Gravity * Time.deltaTime;
                        controllerComponent.CharacterController.Move(moveDirection);
                    }
                    
                    return;
                }

                jumpBody.IsJumped = true;
                _world.Value.GetPool<MovementComponent>().Del(entity);
            }
        }
    }
}