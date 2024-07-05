using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Domain.Components.Interfaces;

namespace Sources.BoundedContexts.BlockMovements.Domain.Components
{
    public struct BlockMovementComponent : ITemporaryComponent
    {
        // public float CurrentDuration;
        // public float Duration;
        public float CurrentTime { get; set; }
        public float Duration { get; set; }
    }
}