using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.Stuns.Domain.Components;
using Sources.BoundedContexts.Stuns.Domain.Events;

namespace Sources.BoundedContexts.EnemyStuns.Infrastructure.Systems
{
    public class EnemyStunParticleSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<EnemyTag, StunParticleComponent, StartStunEvent>> _startStunFilter = default;
        private EcsFilterInject<Inc<EnemyTag, StunParticleComponent, EndStunEvent>> _endStunFilter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _startStunFilter.Value)
            {
                _startStunFilter.Pools.Inc2.Get(entity).ParticleSystem.Play();
            }
            
            foreach (int entity in _endStunFilter.Value)
            {
                _endStunFilter.Pools.Inc2.Get(entity).ParticleSystem.Stop();
            }
        }
    }
}