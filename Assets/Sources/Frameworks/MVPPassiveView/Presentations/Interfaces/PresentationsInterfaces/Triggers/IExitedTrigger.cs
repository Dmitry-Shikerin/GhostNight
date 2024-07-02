using System;

namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Triggers
{
    public interface IExitedTrigger<out T>
    {
        public event Action<T> Exited;
    }
}