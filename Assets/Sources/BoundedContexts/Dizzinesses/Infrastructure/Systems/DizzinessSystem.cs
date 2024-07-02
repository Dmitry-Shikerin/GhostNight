using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.Dizzinesses.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Components;
using UnityEngine;

namespace Sources.BoundedContexts.Dizzinesses.Infrastructure.Systems
{
    public class DizzinessSystem : IEcsRunSystem
    {
        private const float NormalSpeed = 1.7f;
        private const float DizzinessSpeed = 1f;
        private const float StoppingTime = 3f;
        
        private readonly EcsFilterInject<
            Inc<DizzinessComponent, NavMeshComponent>, Exc<CharacterTag>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref NavMeshComponent navMesh = ref _filter.Pools.Inc2.Get(entity);
                ref DizzinessComponent dizziness = ref _filter.Pools.Inc1.Get(entity);

                if (dizziness.CurrentTime >= StoppingTime)
                {
                    dizziness.CurrentTime = 0;
                    navMesh.Agent.speed = NormalSpeed;
                    _filter.Pools.Inc1.Del(entity);
                    
                    continue;
                }

                navMesh.Agent.speed = DizzinessSpeed;
                dizziness.CurrentTime += Time.deltaTime;
            }
        }
    }
}