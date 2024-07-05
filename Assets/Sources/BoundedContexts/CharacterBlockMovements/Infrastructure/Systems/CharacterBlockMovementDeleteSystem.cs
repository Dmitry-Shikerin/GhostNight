using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterBlockMovements.Infrastructure.Systems
{
    public class CharacterBlockMovementDeleteSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, BlockMovementComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref BlockMovementComponent blockMovementComponent = ref _filter.Pools.Inc2.Get(entity);

                blockMovementComponent.CurrentDuration += Time.deltaTime;

                if (blockMovementComponent.CurrentDuration < blockMovementComponent.Duration)
                    continue;
                
                _filter.Pools.Inc2.Del(entity);
            }
        }
    }
}