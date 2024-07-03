using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sources.BoundedContexts.SpawnPoints.Presentation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.RootGameObjects.Presentation
{
    public class RootGameObject : MonoBehaviour
    {
        [FoldoutGroup("SpawnPoints")] 
        [SerializeField] private List<SpawnPoint> _heartSpawnPoints;
        
        public List<SpawnPoint> HeartSpawnPoint => _heartSpawnPoints;
    }
}