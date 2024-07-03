using System;
using JetBrains.Annotations;
using Leopotam.EcsLite;
using Sources.BoundedContexts.Animations.Presentations;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Constructors;
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
        
        private EntityReference _entityReference;

        private void Awake()
        {
            StoppingAnimations.Add(StopWalk);
            StoppingAnimations.Add(StopIdle);
            StoppingAnimations.Add(StopJump);
            StoppingAnimations.Add(StopFlip);
            StoppingAnimations.Add(StopHurt);

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
            
            if (Animator.GetBool(s_isFlip))
                return;
            
            Animator.SetBool(s_isHurt, true);
        }
        
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