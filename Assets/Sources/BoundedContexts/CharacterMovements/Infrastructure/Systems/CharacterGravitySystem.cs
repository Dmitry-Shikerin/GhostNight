using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterControllers.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Gravities.Domain.Components;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterGravitySystem : IEcsRunSystem
    {
        private EcsFilterInject<
            Inc<CharacterTag,
                GravityComponent,
                CharacterControllerComponent>,
            Exc<BlockGravityComponent>> _filter = default;

        private EcsWorldInject _world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref GravityComponent gravityComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterControllerComponent controllerComponent = ref _filter.Pools.Inc3.Get(entity);
            }
        }
    }
}