using System;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct GravityComponent
    {
        [Range(0, 15)]
        public float Gravity;
        [Range(1, 6)]
        public float Speed;
    }
}