using System;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct MovementComponent
    {
        public Transform Transform;
        public CharacterController CharacterController;
        [Range(1, 6)]
        public float Speed;
        [Range(0, 15)]
        public float Gravity;
        public bool IsLockMovement;
    }
}