using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Hammers.Domain.Components;
using Sources.BoundedContexts.Hammers.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.Hammers.Infrastructure.Systems
{
    public class HammerAttackBlockMovementSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<
            Inc<CharacterTag, HammerComponent, StartHummerAttackEvent>> _showFilter = default;
        private readonly EcsFilterInject<
            Inc<CharacterTag, HammerComponent, EndHammerAttackEvent>> _hideFilter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _showFilter.Value)
            {
                if (_world.Value.GetPool<BlockMovementComponent>().Has(entity))
                    continue;
                
                ref BlockMovementComponent blockMovementComponent = 
                    ref _world.Value.GetPool<BlockMovementComponent>().Add(entity);
                
                blockMovementComponent.Duration = 1.2f;
                blockMovementComponent.CurrentTime = 0;
            }
        }
    }
}