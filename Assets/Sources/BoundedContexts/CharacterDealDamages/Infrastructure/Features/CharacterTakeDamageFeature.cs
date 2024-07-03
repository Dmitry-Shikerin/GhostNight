using Sources.BoundedContexts.CharacterDealDamages.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterDealDamages.Infrastructure.Features
{
    public class CharacterTakeDamageFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterTakeDamageSystem());
            AddSystem(new CharacterTakeDamageAnimationSystem());
        }
    }
}