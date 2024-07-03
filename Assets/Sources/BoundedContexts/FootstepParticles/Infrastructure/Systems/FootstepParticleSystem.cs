using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.FootstepParticles.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.FootstepParticles.Infrastructure.Systems
{
    public class FootstepParticleSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<FootstepParticleComponent, DirectionComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref FootstepParticleComponent footstepParticleComponent = ref _filter.Pools.Inc1.Get(entity);
                ref DirectionComponent directionComponent = ref _filter.Pools.Inc2.Get(entity);

                if (directionComponent.Direction != Vector2.zero)
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