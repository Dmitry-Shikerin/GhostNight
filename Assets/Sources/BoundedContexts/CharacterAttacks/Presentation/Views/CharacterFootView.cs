using System;
using Sources.BoundedContexts.CharacterAttacks.Presentation.Triggers;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.BoundedContexts.EnemyAttacks.Presentation.Views;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterAttacks.Presentation.Views
{
    public class CharacterFootView : View
    {
        [SerializeField] private EnemyHeadTrigger _enemyHeadTrigger;

        private void OnEnable() =>
            _enemyHeadTrigger.Entered += OnEnter;

        private void OnDisable() =>
            _enemyHeadTrigger.Entered -= OnEnter;

        private void OnEnter(EnemyHeadView enemyHeadView)
        {
            EntityReference entityReference = enemyHeadView.EntityReference;
            entityReference.World.GetPool<TakeDamageEvent>().Add(entityReference.Entity);
            Debug.Log($"Deal damage");
        }
    }
}