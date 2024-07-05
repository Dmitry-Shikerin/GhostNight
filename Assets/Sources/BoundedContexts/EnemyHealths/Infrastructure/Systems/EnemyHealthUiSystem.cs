using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.Healths.Domain.Components;
using Sources.BoundedContexts.TakeDamages.Domain.Events;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.UI.Images;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyHealths.Infrastructure.Systems
{
    public class EnemyHealthUiSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<EnemyTag, 
                HealthComponent, 
                HealthViewComponent, 
                TakeDamageEvent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                HealthComponent healthComponent = _filter.Pools.Inc2.Get(entity);
                HealthViewComponent healthViewComponent = _filter.Pools.Inc3.Get(entity);
                
                foreach (IImageView imageView in healthViewComponent.HealthView.Images)
                    imageView.Hide();
                
                for (int i = 0; i < healthComponent.Health; i++)
                    healthViewComponent.HealthView.Images[i].Show();
            }
        }
    }
}