using Sources.BoundedContexts.LookAts.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.LookAts.Infrastructure.Features
{
    public class LookAtFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new LookAtSystem());
        }
    }
}