using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyMovements.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;

namespace Sources.BoundedContexts.EnemyMovements.Infrastructure.Systems
{
    public class EnemyMovementSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<EnemyTag, FollowComponent, NavMeshComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref FollowComponent followComponent = ref _filter.Pools.Inc2.Get(entity);
                ref NavMeshComponent navMeshComponent = ref _filter.Pools.Inc3.Get(entity);
                
                navMeshComponent.Agent.destination = followComponent.Target.position;
            }
        }
    }
}