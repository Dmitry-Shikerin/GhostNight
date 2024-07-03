using Sirenix.OdinInspector;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.EnemyAttacks.Presentation.Views
{
    public class EnemyHeadView : View
    {
        [Required] [SerializeField] private EntityReference _entityReference;
        
        public EntityReference EntityReference => _entityReference;
    }
}