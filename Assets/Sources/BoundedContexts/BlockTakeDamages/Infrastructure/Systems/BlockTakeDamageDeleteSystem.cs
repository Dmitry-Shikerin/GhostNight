using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockTakeDamages.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.BlockTakeDamages.Infrastructure.Systems
{
    public class BlockTakeDamageDeleteSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BlockTakeDamageComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref BlockTakeDamageComponent blockTakeDamageComponent = ref _filter.Pools.Inc1.Get(entity);
                
                blockTakeDamageComponent.CurrentTime += Time.deltaTime;
                
                if (blockTakeDamageComponent.CurrentTime < blockTakeDamageComponent.Duration)
                    continue;
                
                _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}