using System;
using Leopotam.EcsLite;
using Sources.BoundedContexts.Dizzinesses.Domain.Components;
using Sources.BoundedContexts.EnemyMovements.Domain.Tags;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Triggers.Presentation;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.Traps.Presentation.Views
{
    public class TrapView : View
    {
        [SerializeField] private EntityTrigger _trigger;
        [SerializeField] private EntityReference _entityReference;
        
        private EcsPool<EnemyTag> _enemyPool;
        private EcsPool<DizzinessComponent> _dizzinessPool;
        
        private EcsPool<EnemyTag> EnemyPool => _enemyPool ??= _entityReference.World.GetPool<EnemyTag>();
        private EcsPool<DizzinessComponent> DizzinessPool => 
            _dizzinessPool ??= _entityReference.World.GetPool<DizzinessComponent>();
        private int _entity;

        private void OnEnable()
        {
            _trigger.Entered += OnEntered;
            _trigger.Exited += OnExited;
        }

        private void OnDisable()
        {
            _trigger.Entered -= OnEntered;
            _trigger.Exited -= OnExited;
        }

        private void OnEntered(EntityReference entityReference)
        {
            if (EnemyPool.Has(entityReference.Entity) == false)
                return;
            
            if (DizzinessPool.Has(entityReference.Entity))
                return;
            
            DizzinessPool.Add(entityReference.Entity);
        }

        private void OnExited(EntityReference entityReference)
        {
        }
    }
}