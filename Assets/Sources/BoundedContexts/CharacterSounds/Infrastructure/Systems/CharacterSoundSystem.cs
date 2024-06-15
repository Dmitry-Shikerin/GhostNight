using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterSounds.Domain.Components;
using UnityEngine;
using Zenject;

namespace Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems
{
    public class CharacterSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterSoundComponent>> _filter = default;
        private EventsBus _eventsBus;
        private DiContainer _container;

        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
        }

        public void Run(IEcsSystems systems)
        {
            if (_eventsBus.HasEventSingleton(out JumpEvent jumpEvent) == false)
                return;

            if (jumpEvent.IsJumped)
                return;
            
            foreach (int entity in _filter.Value)
            {
                Debug.Log($"Play sound");
            }
        }
    }
}