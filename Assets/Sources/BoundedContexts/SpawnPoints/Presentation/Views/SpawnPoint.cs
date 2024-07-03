using Sources.BoundedContexts.SpawnPoints.Presentation.Views.Types;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.SpawnPoints.Presentation.Views
{
    public class SpawnPoint : View
    {
        [SerializeField] private SpawnType _spawnType;
        
        public SpawnType SpawnType => _spawnType;
        public Vector3 Position => transform.position;
    }
}