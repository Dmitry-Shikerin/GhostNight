﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterControllers.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Gravities.Domain.Components;
using Sources.BoundedContexts.Jumps.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class MovementSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag,
                MovementComponent,
                CharacterControllerComponent,
                GravityComponent,
                DirectionComponent>,
            Exc<JumpComponent,
                BlockMovementComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref MovementComponent movementComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterControllerComponent controllerComponent = ref _filter.Pools.Inc3.Get(entity);
                ref GravityComponent gravityComponent = ref _filter.Pools.Inc4.Get(entity);
                ref DirectionComponent directionComponent = ref _filter.Pools.Inc5.Get(entity);

                Vector3 moveDirection =
                    Time.deltaTime
                    * movementComponent.Speed
                    * new Vector3(directionComponent.Direction.x, 0, directionComponent.Direction.y);
                moveDirection.y -= gravityComponent.Gravity * Time.deltaTime;

                controllerComponent.CharacterController.Move(moveDirection);
            }
        }
    }
}