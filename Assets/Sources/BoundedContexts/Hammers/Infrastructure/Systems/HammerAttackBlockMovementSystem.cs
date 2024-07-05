using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.BlockMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Hammers.Domain.Components;
using Sources.BoundedContexts.Hammers.Domain.Events;

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
                _world.Value.GetPool<BlockMovementComponent>().Add(entity);
            }

            foreach (int entity in _hideFilter.Value)
            {
                _world.Value.GetPool<BlockMovementComponent>().Del(entity);
            }
        }
    }
}