﻿using System;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Triggers;
using UnityEngine;

namespace Sources.BoundedContexts.Triggers.Presentation.Common
{
    public abstract class TriggerBase<T> : View, ITrigger<T>
    {
        public event Action<T> Entered;

        public event Action<T> Exited;

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"OnTriggerEnter {other.name}");
            if (other.TryGetComponent(out T component))
            {
                Entered?.Invoke(component);
                
                return;
            }

            if (other.GetComponentInChildren<T>() != null)
            {
                Entered?.Invoke(component);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Debug.Log($"OnTriggerExit {other.name}");
            if (other.TryGetComponent(out T component))
            {
                Exited?.Invoke(component);
                
                return;
            }
            
            if (other.GetComponentInChildren<T>() != null)
            {
                Exited?.Invoke(component);
            }
        }
    }
}