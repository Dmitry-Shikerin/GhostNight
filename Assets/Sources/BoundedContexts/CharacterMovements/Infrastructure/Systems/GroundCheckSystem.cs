﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterControllers.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Jumps.Domain.Components;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class GroundCheckSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<CharacterTag, JumpComponent, CharacterControllerComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref JumpComponent jumpComponent = ref _filter.Pools.Inc2.Get(entity);
                ref CharacterControllerComponent controllerComponent = ref _filter.Pools.Inc3.Get(entity);

                if (jumpComponent.CurrentTime < jumpComponent.UpTime)
                    continue;
                
                if (controllerComponent.CharacterController.isGrounded == false)
                    continue;

                _filter.Pools.Inc2.Del(entity);
            }
        }
    }
}