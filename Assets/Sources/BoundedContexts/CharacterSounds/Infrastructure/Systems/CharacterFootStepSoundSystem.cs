using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;

namespace Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems
{
    public class CharacterFootStepSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, CharacterAnimationComponent, FootstepEvent>> _filter = default;

        private IAudioService _audioService;

        public void Init(IEcsSystems systems)
        {
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                _audioService.PlayAsync(AudioGroupId.Footstep);
            }
        }
    }
}