using Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Interfaces;
using Sources.BoundedContexts.Hearths.Presentation.Views.Implementation;
using Sources.BoundedContexts.Hearths.Presentation.Views.Interfaces;
using Sources.BoundedContexts.Prefabs;
using Sources.Frameworks.GameServices.ObjectPools.Implementation.Objects;
using Sources.Frameworks.GameServices.ObjectPools.Interfaces.Generic;
using Sources.Frameworks.GameServices.Prefabs.Interfaces;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Implementation
{
    public class HeartViewFactory : PoolableObjectFactory<HearthView>, IHeartViewFactory
    {
        public HeartViewFactory(
            IObjectPool<HearthView> pool,
            IPrefabLoader prefabLoader) 
            : base(
                pool,
                prefabLoader)
        {
        }

        public IHearthView Create(HearthView view) =>
            view;

        public IHearthView Create() =>
            CreateView(PrefabPath.Hearth);
    }
}