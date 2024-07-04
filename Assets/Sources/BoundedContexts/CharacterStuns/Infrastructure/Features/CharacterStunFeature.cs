using Sources.BoundedContexts.CharacterStuns.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterStuns.Infrastructure.Features
{
    public class CharacterStunFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterStunSystem());
            AddSystem(new CharacterStunParticleSystem());
        }
    }
}