using Sources.BoundedContexts.Dizzinesses.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Interfaces;

namespace Sources.BoundedContexts.Traps.Infrastructure.Features
{
    public class TrapFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new DizzinessSystem());
        }
    }
}