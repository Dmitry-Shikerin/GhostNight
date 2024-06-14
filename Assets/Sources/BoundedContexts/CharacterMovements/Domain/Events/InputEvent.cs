using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Events
{
    public struct InputEvent : IEventSingleton
    {
        public Vector2 Direction;
    }
}