using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.Dizzinesses.Domain.Components;

namespace Sources.BoundedContexts.Dizzinesses.Infrastructure.Systems
{
    public class DizzinessParticleSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<DizzinessParticlesComponent>> _filter;
        private readonly EcsFilterInject<Inc<DizzinessComponent>> _dizzinessFilter;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref DizzinessParticlesComponent dizzinessParticles = ref _filter.Pools.Inc1.Get(entity);

                if (_dizzinessFilter.Pools.Inc1.Has(entity) == false)
                {
                    if (dizzinessParticles.ParticleSystem.isPlaying == false)
                        continue;
                    
                    dizzinessParticles.ParticleSystem.Stop();
                    
                    continue;
                }
                
                if (dizzinessParticles.ParticleSystem.isPlaying)
                    continue;

                dizzinessParticles.ParticleSystem.Play();
            }
        }
    }
}