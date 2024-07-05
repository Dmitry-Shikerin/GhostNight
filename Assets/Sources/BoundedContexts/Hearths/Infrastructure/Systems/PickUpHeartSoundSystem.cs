using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.Hearths.Domain.Events;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Systems
{
    public class PickUpHeartSoundSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PickUpHearthEvent>> _filter = default;
        
        private IAudioService _audioService;

        public void Init(IEcsSystems systems) =>
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                _audioService.PlayAsync(AudioGroupId.PickUpHeart);
            }
        }
    }
}