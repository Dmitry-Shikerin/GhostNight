using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Sources.Frameworks.GameServices.AddressablesInfr.AssetProviders.Implementation
{
    public abstract class AssetProviderBase
    {
        private List<GameObject> _gameObjects = new List<GameObject>();
        private List<Object> _objects = new List<Object>();
        
        public abstract UniTask LoadAsync();
        
        protected async UniTask<T> LoadAssetAsync<T>(string address) where T : MonoBehaviour
        {
            GameObject asset = await Addressables.LoadAssetAsync<GameObject>(address).Task;
            
            if(asset.TryGetComponent(out T component) == false)
                throw new InvalidOperationException(nameof(component));
            
            _gameObjects.Add(asset);
            
            return component;
        }
        
        protected async UniTask<T> LoadObjectAsync<T>(string address) where T : Object
        {
            Object asset = await Addressables.LoadAssetAsync<Object>(address).Task;
            
            if(asset == null)
                throw new InvalidOperationException(nameof(asset));
            
            if(asset is not T component)
                throw new InvalidOperationException(nameof(asset));
            
            _objects.Add(asset);
            
            return component;
        }

        public void Release()
        {
            _gameObjects.ForEach(Addressables.Release);
            _objects.ForEach(Addressables.Release);
        }
    }
}