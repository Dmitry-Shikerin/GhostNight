using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Domain.Interfaces;
using UnityEngine;

namespace Sources.BoundedContexts.DealDamages.Domain.Events
{
    public struct TakeDamageEvent : IEcsEvent
    {
        public Vector3 From;
        public float CurrentTime;
    }
}