using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
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
            body.Direction = new Vector2(horizontal, vertical);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_eventsBus.HasEventSingleton<JumpEvent>())
                    return;
                
                _eventsBus.NewEventSingleton<JumpEvent>();
            }

        }

        private void DestroyEvent()
        {
            
            if (_eventsBus.HasEventSingleton<JumpEvent>(out var jumpEvent) == false)
                return;

            ref JumpEvent body = ref _eventsBus.GetEventBodySingleton<JumpEvent>();
            
            if (body.Delay == 0)
                body.Delay += Time.deltaTime;
            else if (body.Delay is > 0 and < 3)
                body.Delay += Time.deltaTime;
            else
                _eventsBus.DestroyEventSingleton<JumpEvent>();
        }
    }
}