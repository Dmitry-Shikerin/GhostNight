using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterControllers.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Gravities.Domain.Components;
using Sources.BoundedContexts.Jumps.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class JumpGravitySystem : IEcsRunSystem
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

                if (jumpComponent.CurrentTime < jumpComponent.UpTime)
                    continue;
                
                Vector3 moveDirection =
                    Time.deltaTime
                    * gravityComponent.Speed
                    * new Vector3(directionComponent.Direction.x, 0, directionComponent.Direction.y);
                moveDirection.y -= gravityComponent.Gravity * Time.deltaTime;
                controllerComponent.CharacterController.Move(moveDirection);
            }
        }
    }
}