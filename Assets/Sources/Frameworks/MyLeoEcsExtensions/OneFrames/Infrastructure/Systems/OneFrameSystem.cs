using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Domain.Interfaces;

namespace Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Infrastructure.Systems
{
    public class OneFrameSystem<T> : IEcsRunSystem
        where T : struct, IEcsEvent
    {
        private readonly EcsFilterInject<Inc<T>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                _filter.Pools.Inc1.Del(entity);
            }
        }
    }
}