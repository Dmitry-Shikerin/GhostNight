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
    public class RotateSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, RotateComponent>> _filter = default;
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;


        public void Run(IEcsSystems systems)
        {
            if (_eventsBus.HasEventSingleton(out InputEvent inputEvent) == false)
                return;
            
            if (inputEvent.Direction == Vector2.zero)
                return;

            foreach (int entity in _filter.Value)
            {
                ref RotateComponent rotateComponent = ref _filter.Pools.Inc2.Get(entity);
                Vector3 moveDirection = new Vector3(inputEvent.Direction.x, 0, inputEvent.Direction.y);
                float angle = Vector3.SignedAngle(Vector3.forward, moveDirection, Vector3.up);
                Quaternion targetAngle = Quaternion.Euler(0, angle, 0);
                
                rotateComponent.Transform.rotation = 
                    Quaternion.RotateTowards(
                        rotateComponent.Transform.rotation,
                        targetAngle, 
                        Time.deltaTime * rotateComponent.Speed * 200);
            }
        }
    }
}