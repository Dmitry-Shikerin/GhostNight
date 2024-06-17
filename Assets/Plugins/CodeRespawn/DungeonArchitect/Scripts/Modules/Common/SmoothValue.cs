//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Utils
{
    public abstract class SmoothValue<T>
    {
        protected T targetValue;
        protected T currentValue;
        protected float t = 0;

        public float TimeToArrive = 0.4f;
        public bool HasArrived => t >= 1;
        
        public T Value
        {
            get => currentValue;
            set
            {
                targetValue = value;
                t = 0;
            }
        }

        public T TargetValue => targetValue;

        protected SmoothValue(T value)
        {
            Set(value);
        }

        public void Set(T value)
        {
            currentValue = value;
            targetValue = value;
            t = 0;
        }
        
        public void Update(float deltaTime)
        {
            if (t < 1)
            {
                t += deltaTime / TimeToArrive;
                t = Mathf.Clamp01(t);

                PerformLerp();
            }
        }

        protected abstract void PerformLerp();
    }

    public class SmoothValueFloat : SmoothValue<float>
    {
        public SmoothValueFloat(float value) : base(value) { }
        protected override void PerformLerp()
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, t);
        }
    }
    
    public class SmoothValueVector3 : SmoothValue<Vector3>
    {
        public SmoothValueVector3(Vector3 value) : base(value) { }
        protected override void PerformLerp()
        {
            currentValue = Vector3.Lerp(currentValue, targetValue, t);
        }
    }
    
    public class SmoothValueVector2 : SmoothValue<Vector2>
    {
        public SmoothValueVector2(Vector2 value) : base(value) { }
        protected override void PerformLerp()
        {
            currentValue = Vector2.Lerp(currentValue, targetValue, t);
        }
    }
}