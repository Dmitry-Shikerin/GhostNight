using System;

namespace Sources.BoundedContexts.Healths.Domain.Components
{
    [Serializable]
    public struct HealthComponent
    {
        public int Health;
        public int MaxHealth;
    }
}