using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class BlockJumpRemoveSystem : IEcsRunSystem
    {
        private const float JumpDelay = 1.5f;
        
        private readonly EcsFilterInject<Inc<BlockJumpComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                 ref BlockJumpComponent blockJumpComponent = ref _filter.Pools.Inc1.Get(entity);
                 
                 blockJumpComponent.CurrentTime += Time.deltaTime;
                 
                 if (blockJumpComponent.CurrentTime >= JumpDelay)
                     _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}