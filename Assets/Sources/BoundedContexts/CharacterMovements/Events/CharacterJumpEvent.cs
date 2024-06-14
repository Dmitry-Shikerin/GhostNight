using SevenBoldPencil.EasyEvents;

namespace Sources.BoundedContexts.CharacterMovements.Events
{
    public struct CharacterJumpEvent : IEventSingleton
    {
        public float Delay;
    }
}