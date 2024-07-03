using System;
using Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Interfaces;
using Sources.BoundedContexts.Hearths.Infrastructure.Services.Interfaces;
using Sources.BoundedContexts.Hearths.Presentation.Views.Implementation;
using Sources.BoundedContexts.Hearths.Presentation.Views.Interfaces;
using Sources.Frameworks.GameServices.ObjectPools.Interfaces.Generic;
using UnityEngine;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Services.Implementation
{
    public class HeartSpawnService : IHeartSpawnService
    {
        private readonly IObjectPool<HearthView> _hearthPool;
        private readonly IHeartViewFactory _viewFactory;

        public HeartSpawnService(
            IObjectPool<HearthView> hearthPool,
            IHeartViewFactory viewFactory)
        {
            _hearthPool = hearthPool ?? throw new ArgumentNullException(nameof(hearthPool));
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
        }

        public IHearthView Spawn(Vector3 position)
        {
            IHearthView view = SpawnFromPool() ?? _viewFactory.Create();
            view.Show();

            return view;
        }

        private IHearthView SpawnFromPool()
        {
            HearthView view = _hearthPool.Get<HearthView>();
            
            if (view == null)
                return null;
            
            return _viewFactory.Create(view);
        }
    }
}