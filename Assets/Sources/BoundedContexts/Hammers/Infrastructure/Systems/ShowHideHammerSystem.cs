using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Hammers.Domain.Components;
using Sources.BoundedContexts.Hammers.Domain.Events;

namespace Sources.BoundedContexts.Hammers.Infrastructure.Systems
{
    public class ShowHideHammerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, HammerComponent>> _hammerFilter = default;
        private readonly EcsFilterInject<Inc<CharacterTag, HammerComponent, ShowHammerEvent>> _showFilter = default;
        private readonly EcsFilterInject<Inc<CharacterTag, HammerComponent, HideHammerEvent>> _hideFilter = default;

        public void Init(IEcsSystems systems)
        {
            foreach (int entity in _hammerFilter.Value)
            {
                HammerComponent hammerComponent = _hammerFilter.Pools.Inc2.Get(entity);
                hammerComponent.HammerView.Hide();
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _showFilter.Value)
            {
                HammerComponent hammerComponent = _showFilter.Pools.Inc2.Get(entity);
                hammerComponent.HammerView.Show();
            }

            foreach (int entity in _hideFilter.Value)
            {
                HammerComponent hammerComponent = _hideFilter.Pools.Inc2.Get(entity);
                hammerComponent.HammerView.Hide();
            }
        }
    }
}