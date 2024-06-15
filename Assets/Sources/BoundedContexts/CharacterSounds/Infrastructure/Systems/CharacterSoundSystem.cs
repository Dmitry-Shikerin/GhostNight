using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterSounds.Domain.Components;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems
{
    public class CharacterSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterSoundComponent>> _filter = default;
        private EventsBus _eventsBus;
        private IAudioService _audioService;


        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();
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
                _audioService.Play(AudioClipId.Jump);
            }
        }
    }
}