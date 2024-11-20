using Leopotam.EcsLite;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Temporaries.Domain.Components.Interfaces;

namespace Sources.Frameworks.MyLeoEcsExtensions.Temporaries.Extensions
{
    public static class DeleteTemporaryExtension
    {
        public static IEcsSystems AddTemporary<T>(this IEcsSystems systems) 
            where T : struct, ITemporaryComponent =>
            systems.Add(new TemporarySystem<T>());
    }
}