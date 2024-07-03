using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.Hammers.Presentation.Views
{
    public class HammerView : View
    {
        [SerializeField] private Transform _hitPoint;
        [SerializeField] private float _radius = 2f;
        
        public Vector3 HitPoint => _hitPoint.position;
        public float Radius => _radius;
    }
}