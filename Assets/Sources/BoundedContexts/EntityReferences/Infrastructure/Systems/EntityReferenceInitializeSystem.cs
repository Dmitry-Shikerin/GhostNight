using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EntityReferences.Domain.Components;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;

namespace Sources.BoundedContexts.EntityReferences.Infrastructure.Systems
{
    public class EntityReferenceInitializeSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<EntityReferenceComponent>> _filter = default;
        
        public void Init(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                EntityReferenceComponent entityReferenceComponent = _filter.Pools.Inc1.Get(entity);
                EntityReference entityReference = entityReferenceComponent.EntityReference;
                entityReference.Entity = entity;
                entityReference.World = systems.GetWorld();
            }
        }
    }
}