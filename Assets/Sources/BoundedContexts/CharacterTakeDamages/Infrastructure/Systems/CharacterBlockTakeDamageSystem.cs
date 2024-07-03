using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterTakeDamages.Domain.Componets;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterBlockTakeDamageSystem : IEcsRunSystem
    {
        private const float Delay = 2f;
        
        private readonly EcsFilterInject<Inc<CharacterTag, BlockTakeDamageComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref BlockTakeDamageComponent blockTakeDamageComponent = ref _filter.Pools.Inc2.Get(entity);
                
                blockTakeDamageComponent.CurrentTime += Time.deltaTime;
                
                if (blockTakeDamageComponent.CurrentTime < Delay)
                    continue;
                
                _filter.Pools.Inc2.Del(entity);
            }
        }
    }
}