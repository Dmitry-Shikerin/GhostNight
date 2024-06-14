using SevenBoldPencil.EasyEvents;

namespace Sources.BoundedContexts.CharacterMovements.Domain.Events
{
    public struct JumpEvent : IEventSingleton
    {
        public float Delay;
    }
}