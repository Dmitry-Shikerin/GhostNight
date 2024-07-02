using System;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Cameras.Points;

namespace Sources.Frameworks.GameServices.Cameras.Interfaces
{
    public interface ICameraService
    {
        event Action FollowableChanged;
        
        ICameraFollowable CurrentFollower { get; }
        
        void SetFollower<T>() where T : ICameraFollowable;
        void Add<T>(ICameraFollowable cameraFollowable) where T : ICameraFollowable;
        ICameraFollowable Get<T>() where T : ICameraFollowable;
    }
}