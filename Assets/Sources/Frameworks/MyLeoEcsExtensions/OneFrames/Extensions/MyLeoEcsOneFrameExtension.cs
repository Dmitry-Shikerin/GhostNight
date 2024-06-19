using Leopotam.EcsLite;
using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Domain.Interfaces;
using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Infrastructure.Systems;

namespace Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Extensions
{
    public static class MyLeoEcsOneFrameExtension
    {
        public static IEcsSystem AddOneFrame<T>(this IEcsSystems systems) where T : struct, IEcsEvent =>
            new OneFrameSystem<T>();
    }
}