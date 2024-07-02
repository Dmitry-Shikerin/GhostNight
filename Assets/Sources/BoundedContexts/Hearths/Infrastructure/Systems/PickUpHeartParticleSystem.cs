using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.Hearths.Domain.Components;
using Sources.BoundedContexts.Hearths.Domain.Events;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Systems
{
    public class PickUpHeartParticleSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PickUpHeathParticleComponent, PickUpHearthEvent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                PickUpHeathParticleComponent pickUpHeathParticleComponent = _filter.Pools.Inc1.Get(entity);
                
                pickUpHeathParticleComponent.ParticleSystem.Play();
            }
        }
    }
}