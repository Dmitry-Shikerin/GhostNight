using System;
using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Domain.Components.Interfaces;

namespace Sources.BoundedContexts.Jumps.Domain.Components
{
    [Serializable]
    public struct BlockJumpComponent : ITemporaryComponent
    {
        public float CurrentTime { get; set; }
        public float Duration { get; set; }
    }
}