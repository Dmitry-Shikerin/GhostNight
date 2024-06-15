using Sources.BoundedContexts.Animations.Presentations;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Presentation.Views
{
    public class CharacterAnimation : AnimationViewBase
    {
        private static int s_isWalk = Animator.StringToHash("IsWalk");
        private static int s_isIdle = Animator.StringToHash("IsIdle");
        private static int s_isJump = Animator.StringToHash("IsJump");
        private static int s_isFlip = Animator.StringToHash("IsFlip");

        private void Awake()
        {
            StoppingAnimations.Add(StopWalk);
            StoppingAnimations.Add(StopIdle);
            StoppingAnimations.Add(StopJump);
            StoppingAnimations.Add(StopFlip);
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
        
        public void PlayFlip()
        {
            ExceptAnimation(StopFlip);
            if (Animator.GetBool(s_isFlip))
                return;
                
            Debug.Log($"Play flip");
            Animator.SetBool(s_isFlip, true);
        }

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