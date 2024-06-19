using Leopotam.EcsLite;

namespace Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Interfaces
{
    public interface IEcsFeature : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        void Enable();
        void Disable();
    }
}