using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Healths.Domain.Components;
using Sources.BoundedContexts.TakeDamages.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, HealthComponent, TakeDamageEvent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref HealthComponent healthComponent = ref _filter.Pools.Inc2.Get(entity);

                healthComponent.Health--;
                
                Debug.Log($"Character health take damage, health : {healthComponent.Health}");
            }
        }
    }
}