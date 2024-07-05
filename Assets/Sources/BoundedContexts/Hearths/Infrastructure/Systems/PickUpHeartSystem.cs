using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.Healths.Domain.Components;
using Sources.BoundedContexts.Hearths.Domain.Events;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Systems
{
    public class PickUpHeartSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PickUpHearthEvent, HealthComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref HealthComponent healthComponent = ref _filter.Pools.Inc2.Get(entity);

                if (healthComponent.Health == healthComponent.MaxHealth)
                    continue;
                
                healthComponent.Health++;
            }
        }
    }
}