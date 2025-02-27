﻿using JetBrains.Annotations;
using Leopotam.EcsLite;
using Sources.BoundedContexts.Animations.Presentations;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.BoundedContexts.Hammers.Domain.Events;
using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterAnimations.Presentation.Views
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
        private EcsWorld _world;
        private int _entity;
        
        private EcsWorld World => _world ??= _entityReference.World;
        private int Entity => 
            _entity == default 
            ? _entityReference.Entity 
            : throw new InvalidImplementationException();

        private void Awake()
        {
            _entityReference = GetComponentInParent<EntityReference>();
            
            StoppingAnimations.Add(StopWalk);
            StoppingAnimations.Add(StopIdle);
            StoppingAnimations.Add(StopJump);
            StoppingAnimations.Add(StopFlip);
            StoppingAnimations.Add(StopHurt);
            StoppingAnimations.Add(StopHammerAttack);
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
            World.Invoke<StartHummerAttackEvent>(Entity);
            Animator.SetBool(s_isHammerAttack, true);
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
        private void OnRightStep() =>
            World.Invoke<FootstepEvent>(Entity);

        [UsedImplicitly]
        private void OnLeftStep() =>
            World.Invoke<FootstepEvent>(Entity);

        [UsedImplicitly]
        private void OnStartHammerAttack()
        {
        }

        [UsedImplicitly]
        private void OnEndHammerAttack() =>
            World.Invoke<EndHammerAttackEvent>(Entity);

        [UsedImplicitly]
        private void OnHit() =>
            World.Invoke<HammerAttackHitEvent>(Entity);

        [UsedImplicitly]
        private void OnHurtEnded() =>
            PlayIdle();

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