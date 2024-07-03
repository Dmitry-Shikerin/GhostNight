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
    public class JumpSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, 
                JumpComponent, 
                CharacterControllerComponent, 
                GravityComponent, 
                DirectionComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref JumpComponent jumpComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterControllerComponent controllerComponent = ref _filter.Pools.Inc3.Get(entity);
                ref GravityComponent gravityComponent = ref _filter.Pools.Inc4.Get(entity);
                DirectionComponent directionComponent = _filter.Pools.Inc5.Get(entity);

                if (jumpComponent.CurrentTime >= jumpComponent.UpTime)
                    continue;
                
                jumpComponent.CurrentTime += Time.deltaTime;
                Vector3 direction =
                    Time.deltaTime
                    * new Vector3(
                        directionComponent.Direction.x + 0.2f,
                        jumpComponent.JumpForce,
                        directionComponent.Direction.y + 0.2f);
                controllerComponent.CharacterController.Move(direction);
            }
        }
    }
}