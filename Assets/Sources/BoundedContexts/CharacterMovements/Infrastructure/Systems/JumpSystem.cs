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
        private readonly EcsFilterInject<Inc<CharacterTag, JumpComponent, MovementComponent>> _filter = default;
        
        private EventsBus _eventsBus;
        private MovementComponent _movementComponent;
        private JumpComponent _jumpComponent;

        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
            _movementComponent = _filter.Pools.Inc3.Get(0);
            _jumpComponent = _filter.Pools.Inc2.Get(0);
        }

        public void Run(IEcsSystems systems)
        {
            if (_eventsBus.HasEventSingleton(out JumpEvent inputEvent) == false)
                return;
            
            if (inputEvent.IsJumped)
                return;

            
            
            // float xPosition = _movementComponent.CharacterController.transform.position.x;
            // float zPosition = _movementComponent.CharacterController.transform.position.z;
            //
            // _movementComponent.CharacterController.Move(new Vector3(xPosition, yPosition, zPosition));
            //
        }
    }
}