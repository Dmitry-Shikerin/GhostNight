using Sources.BoundedContexts.Animations.Presentations;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Presentation.Views
{
    public class CharacterAnimation : AnimationViewBase
    {
        private static int s_isWalk = Animator.StringToHash("IsWalk");
        private static int s_isIdle = Animator.StringToHash("IsIdle");
        private static int s_isJump = Animator.StringToHash("IsJump");

        private void Awake()
        {
            StoppingAnimations.Add(StopWalk);
            StoppingAnimations.Add(StopIdle);
            StoppingAnimations.Add(StopJump);
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
            Debug.Log($"Jump");
            Animator.SetBool(s_isJump, true);
        }

        private void StopWalk() =>
            Animator.SetBool(s_isWalk, false);
        
        private void StopIdle() =>
            Animator.SetBool(s_isIdle, false);
        
        private void StopJump() =>
            Animator.SetBool(s_isJump, false);
    }
}