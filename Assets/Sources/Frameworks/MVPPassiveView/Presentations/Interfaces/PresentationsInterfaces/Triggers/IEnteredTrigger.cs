using System;

namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Triggers
{
    public interface IEnteredTrigger<out T>
    {
        public event Action<T> Entered;
    }
}