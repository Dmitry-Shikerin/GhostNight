using JetBrains.Annotations;
using Sources.BoundedContexts.Animations.Presentations;
using Sources.BoundedContexts.CharacterMovements.Domain.Components;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.BoundedContexts.Hammers.Domain.Events;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Presentation.Views
{
    public class CharacterAnimation : AnimationViewBase
    {
        private static int s_isWalk = Animator.StringToHash("IsWalk");
        private static int s_isIdle = Animator.StringToHash("IsIdle");
        private static int s_isJump = Animator.StringToHash("IsJump");
        private static int s_isFlip = Animator.StringToHash("IsFlip");
        private static int s_isHurt = Animator.StringToHash("IsHurt");
        private static int s_isHammerAttack = Animator.StringToHash("IsHummerAttack");
        
        private EntityReference _entityReference;

        private void Awake()
        {
            StoppingAnimations.Add(StopWalk);
            StoppingAnimations.Add(StopIdle);
            StoppingAnimations.Add(StopJump);
            StoppingAnimations.Add(StopFlip);
            StoppingAnimations.Add(StopHurt);
            StoppingAnimations.Add(StopHammerAttack);

            _entityReference = GetComponentInParent<EntityReference>();
        }
        
        public void PlayWalk()
        {
            ExceptAnimation(StopWalk);
            Animator.SetBool(s_isWalk, true);
        }

        public void PlayIdle()
        {
            ExceptAnimation(StopIdle);
            Animator.SetBool(s_isIdle, true);
        }
        
        public void PlayJump()
        {
            ExceptAnimation(StopJump);
            Animator.SetBool(s_isJump, true);
        }

        public void PlayHurt()
        {
            ExceptAnimation(StopHurt);
            
            if (Animator.GetBool(s_isHurt))
                return;
            
            Animator.SetBool(s_isHurt, true);
        }

        public void PlayHammerAttack()
        {
            ExceptAnimation(StopHammerAttack);
            Animator.SetBool(s_isHammerAttack, true);
            _entityReference
                .World
                .GetPool<BlockMovementComponent>()
                .Add(_entityReference.Entity);
        }

        private void StopHammerAttack() =>
            Animator.SetBool(s_isHammerAttack, false);

        public void PlayFlip()
        {
            ExceptAnimation(StopFlip);
            
            if (Animator.GetBool(s_isFlip))
                return;
                
            Animator.SetBool(s_isFlip, true);
        }

        [UsedImplicitly]
        private void OnRightStep()
        {
            _entityReference
                .World
                .GetPool<FootstepEvent>()
                .Add(_entityReference.Entity);
        }

        [UsedImplicitly]
        private void OnLeftStep()
        {
            _entityReference
                .World
                .GetPool<FootstepEvent>()
                .Add(_entityReference.Entity);
        }

        [UsedImplicitly]
        private void OnStartHammerAttack()
        {
            _entityReference
                .World
                .GetPool<ShowHammerEvent>()
                .Add(_entityReference.Entity);
        }

        [UsedImplicitly]
        private void OnEndHammerAttack()
        {
            _entityReference
                .World
                .GetPool<HideHammerEvent>()
                .Add(_entityReference.Entity);
            _entityReference
                .World
                .GetPool<BlockMovementComponent>()
                .Del(_entityReference.Entity);
        }

        [UsedImplicitly]
        private void OnHit()
        {
            
        }

        [UsedImplicitly]
        private void OnHurtEnded()
        {
            _entityReference.World.GetPool<BlockMovementComponent>().Del(_entityReference.Entity);
            PlayIdle();
            Debug.Log($"OnHurtEnded");
        }

        private void StopHurt() =>
            Animator.SetBool(s_isHurt, false);

        private void StopFlip() =>
            Animator.SetBool(s_isFlip, false);

        private void StopWalk() =>
            Animator.SetBool(s_isWalk, false);
        
        private void StopIdle() =>
            Animator.SetBool(s_isIdle, false);
        
        private void StopJump() =>
            Animator.SetBool(s_isJump, false);
    }
}