using Sources.Frameworks.StateMachines.ContextStateMachines.Interfaces.Contexts;
using UnityEngine;

namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Cameras.Points
{
    public interface ICameraFollowable : IContext
    {
        Transform Transform { get; }
    }
}