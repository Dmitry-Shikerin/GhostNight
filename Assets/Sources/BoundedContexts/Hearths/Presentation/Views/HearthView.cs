using Leopotam.EcsLite;
using Sources.BoundedContexts.CharacterMovements.Domain.Tags;
using Sources.BoundedContexts.EntityReferences.Presentation.Views;
using Sources.BoundedContexts.Hearths.Domain.Events;
using Sources.BoundedContexts.Triggers.Presentation;
using Sources.Frameworks.MVPPassiveView.Presentations.Implementation.Views;
using UnityEngine;

namespace Sources.BoundedContexts.Hearths.Presentation.Views
{
    public class HearthView : View
    {
        [SerializeField] private EntityTrigger _trigger;
        [SerializeField] private EntityReference _entityReference;
        
        private EcsPool<CharacterTag> _characterPool;
        private EcsPool<PickUpHearthEvent> _pickUpHearthPool;
        
        private EcsPool<CharacterTag> CharacterPool => 
            _characterPool ??= _entityReference.World.GetPool<CharacterTag>();
        private EcsPool<PickUpHearthEvent> PickUpHearthPool => 
            _pickUpHearthPool ??= _entityReference.World.GetPool<PickUpHearthEvent>();

        private void OnEnable()
        {
            _trigger.Entered += OnEntered;
        }

        private void OnDisable()
        {
            _trigger.Entered -= OnEntered;
        }

        private void OnEntered(EntityReference entityReference)
        {
            int entity = entityReference.Entity;
            
            if (CharacterPool.Has(entity) == false)
                return;
            
            if (PickUpHearthPool.Has(entity))
                return;
            
            PickUpHearthPool.Add(entity);
        }
    }
}