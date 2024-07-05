using Sources.BoundedContexts.CharacterHealths.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterHealths.Infrastructure.Features
{
    public class CharacterHealthFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterHealthUiSystem());
        }
    }
}