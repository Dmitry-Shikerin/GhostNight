using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;

namespace Sources.Frameworks.DoozyWrappers.SignalBuses.Domain.Signals
{
    public struct AudioSignal
    {
        public AudioSignal(AudioGroupId audioClipId)
        {
            AudioClipId = audioClipId;
        }
        
        public AudioGroupId AudioClipId { get; }
    }
}