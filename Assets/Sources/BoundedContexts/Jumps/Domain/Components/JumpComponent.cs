using System;
using UnityEngine;

namespace Sources.BoundedContexts.Jumps.Domain.Components
{
    [Serializable]
    public struct JumpComponent
    {
        [Range(2, 10)]
        public float JumpForce;
        [Range(0, 6)]
        public float UpTime;
        [HideInInspector]
        public float CurrentTime;
        public bool IsPlayJumpSound;
    }
}