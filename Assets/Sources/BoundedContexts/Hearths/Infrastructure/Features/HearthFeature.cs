using Sources.BoundedContexts.Hearths.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Features
{
    public class HearthFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new PickUpHeartParticleSystem());
            AddSystem(new PickUpHeartSoundSystem());
        }
    }
}