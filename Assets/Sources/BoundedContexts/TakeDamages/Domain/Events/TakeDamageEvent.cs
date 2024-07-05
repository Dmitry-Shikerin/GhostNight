using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Domain.Interfaces;
using UnityEngine;

namespace Sources.BoundedContexts.TakeDamages.Domain.Events
{
    public struct TakeDamageEvent : IEcsEvent
    {
        public Vector3 From;
        public float CurrentTime;
    }
}