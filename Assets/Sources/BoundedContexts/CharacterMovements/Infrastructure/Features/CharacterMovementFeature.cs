using Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems;
using Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems;
using Sources.BoundedContexts.FootstepParticles.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Features
{
    public class CharacterMovementFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new CharacterAnimationInitSystem());
            AddSystem(new JumpSystem());
            AddSystem(new MovementSystem());
            AddSystem(new PlayerInputSystem());
            AddSystem(new RotateSystem());
            AddSystem(new GroundCheckSystem());
            AddSystem(new JumpGravitySystem());
            AddSystem(new BlockJumpRemoveSystem());
            AddSystem(new CharacterAnimationSystem());
            AddSystem(new FootstepParticleSystem());
            AddSystem(new CharacterJumpSoundSystem());
            AddSystem(new CharacterFootStepSoundSystem());
        }
    }
}