using Sources.BoundedContexts.Hammers.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.Hammers.Infrastructure.Features
{
    public class HammerFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new ShowHideHammerSystem());
            AddSystem(new HummerAttackSystem());
        }
    }
}