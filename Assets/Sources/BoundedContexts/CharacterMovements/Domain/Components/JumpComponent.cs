using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
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