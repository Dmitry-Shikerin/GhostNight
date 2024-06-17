using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.Cameras.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;

namespace Sources.BoundedContexts.Cameras.Infrastructure.Systems
{
    public class VirtualCameraInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CharacterTag, TransformComponent>> _transformFilter = default;
        private readonly EcsFilterInject<Inc<VirtualCameraComponent>> _virtualCameraFilter = default;
        
        public void Init(IEcsSystems systems)
        {
            if (_virtualCameraFilter.Value.GetEntitiesCount() > 1)
                throw new ArgumentOutOfRangeException("VirtualCamera");

            ref VirtualCameraComponent virtualCameraComponent = ref _virtualCameraFilter.Pools.Inc1.Get(0);
            ref TransformComponent transformComponent = ref _transformFilter.Pools.Inc2.Get(0);

            virtualCameraComponent.VirtualCamera.Follow = transformComponent.Transform;
        }
    }
}