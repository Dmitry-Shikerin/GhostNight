using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.FootstepParticles.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.FootstepParticles.Infrastructure.Systems
{
    public class FootstepParticleSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<FootstepParticleComponent>> _filter = default;
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {
            if (_eventsBus.HasEventSingleton(out InputEvent inputEvent) == false)
                return;

            foreach (int entity in _filter.Value)
            {
                ref FootstepParticleComponent footstepParticleComponent = ref _filter.Pools.Inc1.Get(entity);

                if (inputEvent.Direction != Vector2.zero)
                {
                    if (footstepParticleComponent.ParticleSystem.isPlaying)
                        return;

                    footstepParticleComponent.ParticleSystem.Play();
                }
                else
                {
                    if (footstepParticleComponent.ParticleSystem.isPlaying == false)
                        return;
                    
                    footstepParticleComponent.ParticleSystem.Stop();
                }
            }
        }
    }
}