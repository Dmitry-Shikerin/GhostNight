using System;
using System.Collections.Generic;
using Sources.Frameworks.GameServices.Cameras.Interfaces;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Cameras.Points;

namespace Sources.Frameworks.GameServices.Cameras.Implementation
{
    public class CameraService : ICameraService
    {
        private Dictionary<Type, ICameraFollowable> _cameraTargets = new Dictionary<Type, ICameraFollowable>();

        public event Action FollowableChanged;
        
        public ICameraFollowable CurrentFollower { get; private set; }

        public void SetFollower<T>() where T : ICameraFollowable
        {
            if (_cameraTargets.ContainsKey(typeof(T)) == false)
                throw new InvalidOperationException(nameof(T));
            
            CurrentFollower = _cameraTargets[typeof(T)];
            FollowableChanged?.Invoke();
        }

        public void Add<T>(ICameraFollowable cameraFollowable) where T : ICameraFollowable
        {
            if (_cameraTargets.ContainsKey(typeof(T)))
                throw new InvalidOperationException(nameof(T));
            
            _cameraTargets[typeof(T)] = cameraFollowable;
        }

        public ICameraFollowable Get<T>() where T : ICameraFollowable
        {
            if (_cameraTargets.ContainsKey(typeof(T)) == false)
                throw new InvalidOperationException(nameof(T));

            return _cameraTargets[typeof(T)];
        }
    }
}