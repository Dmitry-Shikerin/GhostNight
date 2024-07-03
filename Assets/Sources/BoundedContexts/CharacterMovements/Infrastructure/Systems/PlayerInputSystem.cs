using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class PlayerInputSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag>, Exc<BlockJumpComponent>> _filter;
        private readonly EcsFilterInject<Inc<CharacterTag, DirectionComponent>> _directionFilter;
        private readonly EcsWorldInject _world;
        
        public void Run(IEcsSystems systems)
        {
            UpdateDirectionInput();
            UpdateJumpInput();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateDirectionInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            foreach (int entity in _directionFilter.Value)
            {
                ref DirectionComponent directionComponent = ref _directionFilter.Pools.Inc2.Get(entity);
                directionComponent.Direction = new Vector2(horizontal, vertical).normalized;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (int entity in _filter.Value)
                {
                    ref JumpComponent jumpComponent = ref _world.Value.GetPool<JumpComponent>().Add(entity);
                    _world.Value.GetPool<BlockJumpComponent>().Add(entity);
                    jumpComponent.JumpForce = 10f;
                    jumpComponent.UpTime = 0.3f;
                }
            }
        }
    }
}