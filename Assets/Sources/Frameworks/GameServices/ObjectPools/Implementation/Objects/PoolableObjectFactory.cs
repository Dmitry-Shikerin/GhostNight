using System;
using JetBrains.Annotations;
using Sources.Frameworks.GameServices.ObjectPools.Interfaces.Generic;
using Sources.Frameworks.GameServices.Prefabs.Interfaces;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sources.Frameworks.GameServices.ObjectPools.Implementation.Objects
{
    public abstract class PoolableObjectFactory<T>
        where T : MonoBehaviour, IView
    {
        private readonly IObjectPool<T> _pool;
        private readonly IPrefabLoader _prefabLoader;

        protected PoolableObjectFactory(IObjectPool<T> pool, IPrefabLoader prefabLoader)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _prefabLoader = prefabLoader ?? throw new ArgumentNullException(nameof(prefabLoader));
        }
        
        protected T CreateView(string prefabPath)
        {
            T enemyView = Object.Instantiate(
                _prefabLoader.Load<T>(prefabPath));

            enemyView
                .AddComponent<PoolableObject>()
                .SetPool(_pool);
            
            _pool.AddToCollection(enemyView);

            return enemyView;
        }
    }
}