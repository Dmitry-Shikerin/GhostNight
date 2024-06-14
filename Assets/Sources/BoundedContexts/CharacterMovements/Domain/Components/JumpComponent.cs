using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct JumpComponent
    {
        [Range(0, 6)]
        public float Height;
        [Range(0, 6)]
        public float Delay;
        public Vector3 TargetPosition;
    }
}