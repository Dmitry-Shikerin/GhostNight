using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.LookAts.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.LookAts.Infrastructure.Systems
{
    public class LookAtSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<LookAtComponent>> _filter = default;
        
        private Camera _mainCamera;

        public void Init(IEcsSystems systems) =>
            _mainCamera = Camera.main;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref LookAtComponent lookAtComponent = ref _filter.Pools.Inc1.Get(entity);
                
                Quaternion rotation = _mainCamera.transform.rotation;
                lookAtComponent.Transform.LookAt(
                    lookAtComponent.Transform.position + rotation * Vector3.back, 
                    rotation * Vector3.up);
            }
        }
    }
}