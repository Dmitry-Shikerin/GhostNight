using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.Stuns.Domain.Components;
using Sources.BoundedContexts.Stuns.Domain.Events;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyStuns.Infrastructure.Systems
{
    public class EnemyStunAudioSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilterInject<Inc<EnemyTag, StunParticleComponent, StartStunEvent>> _startStunFilter = default;
        private EcsFilterInject<Inc<EnemyTag, StunParticleComponent, EndStunEvent>> _endStunFilter = default;
        
        private IAudioService _audioService;

        public void Init(IEcsSystems systems) =>
            _audioService = systems.GetShared<SharedData>().DiContainer.Resolve<IAudioService>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _startStunFilter.Value)
            {
                _audioService.PlayAsync(AudioGroupId.Stun);
            }

            foreach (int entity in _endStunFilter.Value)
            {
                _audioService.Stop(AudioGroupId.Stun);
                Debug.Log($"Stop stun sound");
            }
        }
    }
}