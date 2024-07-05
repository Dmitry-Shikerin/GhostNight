using Leopotam.EcsLite;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Domain.Components.Interfaces;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Infrastructure.Systems;

namespace Sources.Frameworks.MyLeoEcsExtensions.Temporaries.Extensions
{
    public static class DeleteTemporaryExtension
    {
        public static IEcsSystems AddDeleteAfterTime<T>(this IEcsSystems systems) 
            where T : struct, ITemporaryComponent =>
            systems.Add(new TemporarySystem<T>());
    }
}