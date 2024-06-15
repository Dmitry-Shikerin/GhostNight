using SevenBoldPencil.EasyEvents;
using Zenject;

namespace Sources.App.Ecs
{
    public class SharedData
    {
        public EventsBus EventsBus { get; set; }
        public DiContainer DiContainer { get; set; }
    }
}