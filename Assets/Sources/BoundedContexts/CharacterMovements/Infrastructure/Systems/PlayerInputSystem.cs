using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag>, Exc<BlockJumpComponent>> _filter;
        private readonly EcsWorldInject _world;
        
        private EventsBus _eventsBus;
        
        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
            _eventsBus.NewEventSingleton<InputEvent>();
        }

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

            ref InputEvent body = ref _eventsBus.GetEventBodySingleton<InputEvent>();
            body.Direction = new Vector2(horizontal, vertical).normalized;
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
                    Debug.Log(jumpComponent);
                    jumpComponent.JumpForce = 10f;
                    jumpComponent.UpTime = 0.3f;
                    _eventsBus.NewEventSingleton<JumpEvent>();
                }
            }
        }
    }
}