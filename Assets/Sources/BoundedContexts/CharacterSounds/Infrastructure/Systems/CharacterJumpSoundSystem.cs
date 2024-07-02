using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterSounds.Domain.Components;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;

namespace Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems
{
    public class CharacterJumpSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterSoundComponent>> _filter = default;
        private EventsBus _eventsBus;
        private IAudioService _audioService;


        public void Init(IEcsSystems systems)
        {
            _eventsBus = systems.GetShared<SharedData>().EventsBus;
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();
        }

        public void Run(IEcsSystems systems)
        {
            // foreach (int entity in _filter.Value)
            //     _audioService.Play(AudioClipId.Jump);
        }
    }
}