using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class CharacterAnimationInitSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<CharacterAnimationComponent> _characterAnimationPool = default;
        
        public void Init(IEcsSystems systems)
        {
            // Object
            //     .FindObjectOfType<CharacterAnimation>()
            //     .Construct(_world.Value, entity);
        }
    }
}