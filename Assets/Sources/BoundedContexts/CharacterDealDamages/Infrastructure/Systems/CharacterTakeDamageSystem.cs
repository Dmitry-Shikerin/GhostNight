using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterDealDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, TakeDamageEvent>> _filter;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                Debug.Log($"Character Deal damage");
            }
        }
    }
}