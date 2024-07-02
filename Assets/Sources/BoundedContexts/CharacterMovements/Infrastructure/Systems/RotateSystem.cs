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
    public class RotateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, RotateComponent, DirectionComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {

            foreach (int entity in _filter.Value)
            {
                ref RotateComponent rotateComponent = ref _filter.Pools.Inc2.Get(entity);
                ref DirectionComponent directionComponent = ref _filter.Pools.Inc3.Get(entity);
                
                if (directionComponent.Direction == Vector2.zero)
                    continue;
                
                Vector3 moveDirection = new Vector3(directionComponent.Direction.x, 0, directionComponent.Direction.y);
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