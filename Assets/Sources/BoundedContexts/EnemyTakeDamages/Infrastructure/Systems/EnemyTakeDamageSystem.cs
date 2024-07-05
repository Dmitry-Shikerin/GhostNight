using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.Healths.Domain.Components;
using Sources.BoundedContexts.TakeDamages.Domain.Events;

namespace Sources.BoundedContexts.EnemyTakeDamages.Infrastructure.Systems
{
    public class EnemyTakeDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<EnemyTag, 
                HealthComponent, 
                TakeDamageEvent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref HealthComponent healthComponent = ref _filter.Pools.Inc2.Get(entity);
                
                healthComponent.Health--;
            }
        }
    }
}