using Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Features
{
    public class CharacterTakeDamageFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterTakeDamageBlockMovementSystem());
            AddSystem(new CharacterTakeDamageAnimationSystem());
            AddSystem(new CharacterTakeDamageParticleSystem());
            AddSystem(new CharacterTakeDamageAudioSystem());
            AddSystem(new CharacterBlockTakeDamageSystem());
        }
    }
}