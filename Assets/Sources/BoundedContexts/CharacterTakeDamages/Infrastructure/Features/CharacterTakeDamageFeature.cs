using Sources.BoundedContexts.CharacterDealDamages.Infrastructure.Systems;
using Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Features
{
    public class CharacterTakeDamageFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterTakeDamageAnimationSystem());
            AddSystem(new CharacterTakeDamageParticleSystem());
            AddSystem(new CharacterTakeDamageAudioSystem());
            AddSystem(new CharacterBlockTakeDamageSystem());
        }
    }
}