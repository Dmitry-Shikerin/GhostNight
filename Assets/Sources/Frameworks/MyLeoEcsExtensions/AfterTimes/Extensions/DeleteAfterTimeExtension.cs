using Leopotam.EcsLite;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Domain.Components.Interfaces;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Infrastructure.Systems;

namespace Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Extensions
{
    public static class DeleteAfterTimeExtension
    {
        public static IEcsSystems AddDeleteAfterTime<T>(this IEcsSystems systems) 
            where T : struct, IAfterTimeDeleteComponent =>
            systems.Add(new DeleteAfterTime<T>());
    }
}