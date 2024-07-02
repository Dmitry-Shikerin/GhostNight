using UnityEngine;

namespace Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Cameras
{
    public interface ICinemachineCameraView : IView
    {
        void Follow(Transform target);
    }
}