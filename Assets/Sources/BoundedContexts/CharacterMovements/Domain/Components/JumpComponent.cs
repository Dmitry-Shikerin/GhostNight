using System;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct JumpComponent
    {
        [Range(2, 10)]
        public float JumpForce;
        [Range(0, 6)]
        public float Delay;
        [HideInInspector]
        public float CurrentDalay;
        public Vector3 TargetPosition;
    }
}