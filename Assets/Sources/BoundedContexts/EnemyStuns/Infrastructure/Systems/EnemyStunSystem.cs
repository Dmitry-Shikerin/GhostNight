using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.Stuns.Domain.Components;
using Sources.BoundedContexts.Stuns.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyStuns.Infrastructure.Systems
{
    public class EnemyStunSystem : IEcsRunSystem
    {
        private const float Duration = 2f;
        
        private readonly EcsFilterInject<Inc<EnemyTag, StunComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref StunComponent stunComponent = ref _filter.Pools.Inc2.Get(entity);

                if (Mathf.Approximately(stunComponent.CurrentTime, 0))
                    systems.GetWorld().GetPool<StartStunEvent>().Add(entity);

                if (stunComponent.CurrentTime >= Duration)
                {
                    _filter.Pools.Inc2.Del(entity);
                    systems.GetWorld().GetPool<EndStunEvent>().Add(entity);
                }
                
                stunComponent.CurrentTime += Time.deltaTime;
            }
        }
    }
}