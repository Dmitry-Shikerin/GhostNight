using System;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Components
{
    [Serializable]
    public struct BlockJumpComponent
    {
        public float CurrentTime;
        public float MaxTime;
    }
}