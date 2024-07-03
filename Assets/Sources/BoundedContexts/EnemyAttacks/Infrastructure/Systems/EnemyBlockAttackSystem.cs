using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyAttacks.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyAttacks.Infrastructure.Systems
{
    public class EnemyBlockAttackSystem : IEcsRunSystem
    {
        private const float TimeToLock = 3f;
        
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<EnemyTag, AttackComponent, BlockAttackComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref BlockAttackComponent blockAttackComponent = ref _filter.Pools.Inc3.Get(entity);

                blockAttackComponent.CurrentTime += Time.deltaTime;

                if (blockAttackComponent.CurrentTime >= TimeToLock)
                    _world.Value.GetPool<BlockAttackComponent>().Del(entity);
            }
        }
    }
}