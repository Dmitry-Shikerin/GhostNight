using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Hammers.Domain.Components;
using Sources.BoundedContexts.Hammers.Domain.Events;
using Sources.BoundedContexts.Layers.Domain.Const;
using Sources.BoundedContexts.Stuns.Domain.Components;
using Sources.Frameworks.GameServices.Overlaps.Interfaces;

namespace Sources.BoundedContexts.Hammers.Infrastructure.Systems
{
    public class HammerAttackSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<CharacterTag, HammerComponent, HammerAttackHitEvent>> _filter = default;
        private IOverlapService _overlapService;

        public void Init(IEcsSystems systems) =>
            _overlapService = systems.GetShared<SharedData>().DiContainer.Resolve<IOverlapService>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                HammerComponent hummerComponent = _filter.Pools.Inc2.Get(entity);

                IReadOnlyList<EntityReference> entityReferences =
                    _overlapService.OverlapSphere<EntityReference>(
                        hummerComponent.HammerView.HitPoint,
                        hummerComponent.HammerView.Radius,
                        LayerConst.Enemy);

                if (entityReferences.Count <= 0)
                    continue;

                foreach (EntityReference entityReference in entityReferences)
                {
                    _world.Value.GetPool<TakeDamageEvent>().Add(entityReference.Entity);

                    if (_world.Value.GetPool<StunComponent>().Has(entityReference.Entity))
                        continue;

                    _world.Value.GetPool<StunComponent>().Add(entityReference.Entity).CurrentTime = 0;
                }
            }
        }
    }
}