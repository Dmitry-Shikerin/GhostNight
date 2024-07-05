using Sources.Frameworks.MyLeoEcsExtensions.AfterTimes.Domain.Components.Interfaces;

namespace Sources.BoundedContexts.BlockTakeDamages.Domain.Components
{
    public struct BlockTakeDamageComponent : ITemporaryComponent
    {
        public float CurrentTime { get; set; }
        public float Duration { get; set; }
    }
}