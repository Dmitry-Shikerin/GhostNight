using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Stuns.Domain.Components;

namespace Sources.BoundedContexts.CharacterStuns.Infrastructure.Systems
{
    public class CharacterStunParticleSystem : IEcsRunSystem
    {
        private const float Duration = 1f;
        
        private readonly EcsFilterInject<Inc<CharacterTag, StunParticleComponent, StunComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                
            }
        }
    }
}