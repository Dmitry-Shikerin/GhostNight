﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class MovementSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, MovementComponent>> _filter = default;
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {
            if (_eventsBus.HasEventSingleton(out InputEvent inputEvent) == false)
                return;

            foreach (int entity in _filter.Value)
            {
                ref MovementComponent movementComponent = ref _filter.Pools.Inc2.Get(entity);
                Vector3 moveDirection = 
                    Time.deltaTime 
                    * movementComponent.Speed 
                    * new Vector3(inputEvent.Direction.x, 0, inputEvent.Direction.y);
                movementComponent.CharacterController.Move(moveDirection);
            }
        }
    }
}