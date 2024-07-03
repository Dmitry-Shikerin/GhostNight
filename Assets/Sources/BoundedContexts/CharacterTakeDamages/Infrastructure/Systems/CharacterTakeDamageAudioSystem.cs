using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageAudioSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, TakeDamageEvent>> _filter = default;
        
        private IAudioService _audioService;

        public void Init(IEcsSystems systems) =>
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                _audioService.PlayAsync(AudioGroupId.CharacterHurt);
            }
        }
    }
}