using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Systems
{
    public class PlayerJumpSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _eventsBus.NewEventSingleton<CharacterJumpEvent>();

            if (_eventsBus.HasEventSingleton<CharacterJumpEvent>(out var jumpEvent))
            {
                if (jumpEvent.Delay == 0)
                {
                    Debug.Log($"Jump {jumpEvent.Delay}");
                    ref CharacterJumpEvent body = ref _eventsBus.GetEventBodySingleton<CharacterJumpEvent>();
                    body.Delay += Time.deltaTime;
                }
                else if (jumpEvent.Delay is > 0 and < 3)
                {
                    ref CharacterJumpEvent body = ref _eventsBus.GetEventBodySingleton<CharacterJumpEvent>();
                    body.Delay += Time.deltaTime;
                    Debug.Log($"Delay {jumpEvent.Delay}");
                }
                else
                {
                    Debug.Log($"Destroy {jumpEvent.Delay}");
                    _eventsBus.DestroyEventSingleton<CharacterJumpEvent>();
                }
            }
        }
    }
}