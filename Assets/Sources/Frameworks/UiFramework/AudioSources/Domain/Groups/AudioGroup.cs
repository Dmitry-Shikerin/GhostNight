using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sources.Frameworks.MVPPassiveView.Controllers.Interfaces.ControllerLifetimes;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Constructors;
using Sources.Frameworks.UiFramework.AudioSources.Domain.Configs;
using Sources.Frameworks.UiFramework.AudioSources.Domain.Groups.Types;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;
using Sources.Frameworks.Utils.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Frameworks.UiFramework.AudioSources.Domain.Groups
{
    [Serializable]
    public class AudioGroup : ScriptableObject,IConstruct<IAudioService>, IDestroy
    {
        [SerializeField] private AudioGroupId _id;
        [SerializeField] private AudioServiceDataBase _parent;
        [FormerlySerializedAs("_playingType")]
        [EnumToggleButtons] [Space(10)]
        [SerializeField] private PlayingType type;
        [Space(10)]
        [SerializeField] private List<AudioClip> _audioClips;
        [BoxGroup("CurrentClip")] [HideLabel]
        [SerializeField] private AudioClip _currentClip;
        [BoxGroup("CurrentClip", showLabel: false)]
        [ProgressBar(0, 100, r: 255f, g: 215f, b: 0.5f, Height = 10, DrawValueLabel = false)]
        [HideLabel]
        [SerializeField] private float _currentTime;

        private IAudioService _audioService;

        public PlayingType Type => type;
        public IReadOnlyList<AudioClip> AudioClips => _audioClips;
        public bool IsPlaying { get; private set; } = false;
        public AudioClip CurrentClip => _currentClip;
        public AudioGroupId Id => _id;

        public float CurrentTime => _currentTime;

        public void Construct(IAudioService soundService) =>
            _audioService = soundService ?? throw new ArgumentNullException(nameof(soundService));

        public void Destroy()
        {
            _audioService = null;
            _currentClip = null;
            _currentTime = 0;
        }
        
        public void SetId(AudioGroupId id) =>
            _id = id;

        public void Play() =>
            IsPlaying = true;

        public void Stop() =>
            IsPlaying = false;

        public void SetCurrentClip(AudioClip clip) =>
            _currentClip = clip;

        public void SetCurrentTime(float time) =>
            _currentTime = time.FloatToPercent(_currentClip.length);

        [ButtonGroup("CurrentClip/Buttons")] 
        [Button(SdfIconType.ArrowLeft)] 
        [HideLabel]
        private void PreviousClip()
        {
            
        }

        [Button(SdfIconType.Pause)]
        [ButtonGroup("CurrentClip/Buttons")] 
        [HideLabel] 
        private void PauseClip()
        {
            
        }

        [ButtonGroup("CurrentClip/Buttons")] 
        [Button(SdfIconType.ArrowRight)] 
        [HideLabel]
        private void NextClip()
        {
            
        }

        [PropertySpace(10)]
        [Button(ButtonSizes.Large)] 
        private void Remove() =>
            _parent.RemoveGroup(this);

        public void SetDataBase(AudioServiceDataBase audioServiceDataBase) =>
            _parent = audioServiceDataBase ?? throw new ArgumentNullException(nameof(audioServiceDataBase));
    }
}