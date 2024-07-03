using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterTakeDamages.Domain.Componets;
using Sources.BoundedContexts.DealDamages.Domain.Events;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageParticleSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, TakeDamageParticleComponent, TakeDamageEvent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                TakeDamageParticleComponent takeDamageParticleComponent = _filter.Pools.Inc2.Get(entity);

                if (takeDamageParticleComponent.ParticleSystem.isPlaying)
                    continue;
                
                takeDamageParticleComponent.ParticleSystem.Play();
            }
        }
    }
}