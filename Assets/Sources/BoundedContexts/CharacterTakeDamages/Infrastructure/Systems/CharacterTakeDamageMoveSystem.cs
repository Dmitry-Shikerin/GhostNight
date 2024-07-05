using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterControllers.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.CharacterTakeDamages.Domain.Componets;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems
{
    public class CharacterTakeDamageMoveSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _ecsWorld = default;

        private readonly EcsFilterInject<
            Inc<CharacterTag,
                CharacterControllerComponent,
                TransformComponent,
                RigidBodyComponent,
                TakeDamageEvent,
                TakeDamageAnimationCurveComponent>> _filter;

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref CharacterControllerComponent characterControllerComponent = ref _filter.Pools.Inc2.Get(entity);
                ref TransformComponent transformComponent = ref _filter.Pools.Inc3.Get(entity);
                ref TakeDamageEvent takeDamageEvent = ref _filter.Pools.Inc5.Get(entity);
                ref RigidBodyComponent rigidBodyComponent = ref _filter.Pools.Inc4.Get(entity);
                ref TakeDamageAnimationCurveComponent takeDamageAnimationCurveComponent = 
                    ref _filter.Pools.Inc6.Get(entity);

                takeDamageEvent.CurrentTime += Time.deltaTime;
                float value = takeDamageAnimationCurveComponent.AnimationCurve.Evaluate(takeDamageEvent.CurrentTime);
                
                Vector3 direction = transformComponent.Transform.position
                                    - takeDamageEvent.From;
                direction.y += value;
                // Debug.Log(direction);
                characterControllerComponent.CharacterController.Move(direction);
                Debug.Log(direction);
                // _ecsWorld.Value.GetPool<TakeDamageEvent>().Del(entity);
            }
        }
    }
}