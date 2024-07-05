using System;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct MovementComponent
    {
        [Range(1, 6)]
        public float Speed;
    }
}