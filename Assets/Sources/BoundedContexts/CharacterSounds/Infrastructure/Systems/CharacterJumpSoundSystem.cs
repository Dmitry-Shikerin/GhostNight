using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterSounds.Domain.Components;
using Sources.BoundedContexts.Jumps.Domain.Components;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;

namespace Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems
{
    public class CharacterJumpSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterSoundComponent, JumpComponent>> _filter = default;
        private IAudioService _audioService;


        public void Init(IEcsSystems systems) =>
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref JumpComponent jumpComponent = ref _filter.Pools.Inc2.Get(entity);

                if (jumpComponent.IsPlayJumpSound)
                    continue;
                
                jumpComponent.IsPlayJumpSound = true;
                _audioService.PlayAsync(AudioGroupId.CharacterJump);
            }
        }
    }
}