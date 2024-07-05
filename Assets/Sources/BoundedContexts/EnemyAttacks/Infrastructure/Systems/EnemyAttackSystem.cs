using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Presentation.Converters;
using Sources.BoundedContexts.CharacterTakeDamages.Domain.Componets;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.BoundedContexts.EnemyAttacks.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyAttacks.Infrastructure.Systems
{
    public class EnemyAttackSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;

        private readonly EcsFilterInject<
            Inc<EnemyTag,
                AttackComponent,
                NavMeshComponent>,
            Exc<BlockAttackComponent>> _filter = default;
        private readonly EcsFilterInject<Inc<TransformComponent>> _transformFilter = default;

        private EntityReference _characterReference;

        public void Init(IEcsSystems systems)
        {
            _characterReference = Object.FindObjectOfType<CharacterTagConverter>().GetComponent<EntityReference>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref NavMeshComponent navMeshComponent = ref _filter.Pools.Inc3.Get(entity);
                    TransformComponent characterTransformComponent = 
                        _transformFilter.Pools.Inc1.Get(_characterReference.Entity);

                float distance = Vector3.Distance(
                    characterTransformComponent.Transform.position, navMeshComponent.Agent.transform.position);

                if (distance <= navMeshComponent.Agent.stoppingDistance + 0.1f)
                {
                    if (_world.Value.GetPool<BlockTakeDamageComponent>().Has(_characterReference.Entity))
                        continue;
                    
                    if (_world.Value.GetPool<TakeDamageEvent>().Has(_characterReference.Entity))
                        continue;

                    ref TakeDamageEvent takeDamageEvent =  
                        ref _world.Value.GetPool<TakeDamageEvent>().Add(_characterReference.Entity);
                    takeDamageEvent.From = navMeshComponent.Agent.transform.position;
                    _world.Value.GetPool<BlockAttackComponent>().Add(entity);
                }
            }
        }
    }
}