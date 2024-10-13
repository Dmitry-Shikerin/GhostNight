using Sources.Frameworks.MyLeoEcsExtensions.Temporaries.Domain.Components.Interfaces;

namespace Sources.BoundedContexts.TakeDamages.Domain.Components
{
    public struct BlockTakeDamageComponent : ITemporaryComponent
    {
        public float CurrentTime { get; set; }
        public float Duration { get; set; }
    }
}