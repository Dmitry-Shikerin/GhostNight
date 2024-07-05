using System;
using UnityEngine;

namespace Sources.BoundedContexts.Healths.Domain
{
    public class Health
    {
        private int _value;

        public event Action HealthChanged;
        public event Action Died;

        public int Value
        {
            get => _value;
            private set
            {
                _value = Mathf.Clamp(value, 0, 5);

                if (value != _value)
                    HealthChanged?.Invoke();
            }
        }

        public void Increase() =>
            Value++;

        public void Decrease()
        {
            Value--;

            if (Value <= 0)
                Died?.Invoke();
        }
    }
}