using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.TakeDamages.Domain.Events;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageBlockMovementSystem : IEcsRunSystem
    {
        private const float Duration = 1.3f;
        private readonly EcsFilterInject<Inc<CharacterTag, TakeDamageEvent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref BlockMovementComponent blockMovementComponent = 
                    ref systems.GetWorld().GetPool<BlockMovementComponent>().Add(entity);

                blockMovementComponent.Duration = Duration;
            }
        }
    }
}