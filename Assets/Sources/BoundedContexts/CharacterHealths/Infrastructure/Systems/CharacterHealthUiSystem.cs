using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Healths.Domain.Components;
using Sources.BoundedContexts.Healths.Presentation.Implementation;
using Sources.BoundedContexts.Hearths.Domain.Events;
using Sources.BoundedContexts.Huds.Presentations;
using Sources.BoundedContexts.TakeDamages.Domain.Events;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.UI.Images;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterHealths.Infrastructure.Systems
{
    public class CharacterHealthUiSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, HealthComponent, TakeDamageEvent>> _filter = default;
        private readonly EcsFilterInject<Inc<
            CharacterTag, HealthComponent, PickUpHearthEvent>> _pickUpHeartFilter = default;
        
        private HealthView _healthView;

        public void Init(IEcsSystems systems) =>
            _healthView = systems.GetShared<SharedData>().DiContainer.Resolve<GameplayHud>().CharacterHealthView;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                HealthComponent healthComponent = _filter.Pools.Inc2.Get(entity);
                
                foreach (IImageView imageView in _healthView.Images)
                    imageView.Hide();
                
                for (int i = 0; i < healthComponent.Health; i++)
                    _healthView.Images[i].Show();
            }
            
            foreach (int entity in _pickUpHeartFilter.Value)
            {
                HealthComponent healthComponent = _pickUpHeartFilter.Pools.Inc2.Get(entity);
                
                foreach (IImageView imageView in _healthView.Images)
                    imageView.Hide();
                
                for (int i = 0; i < healthComponent.Health; i++)
                    _healthView.Images[i].Show();
            }
        }
    }
}