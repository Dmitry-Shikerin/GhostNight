using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sources.Frameworks.GameServices.ObjectPools.Implementation;
using Sources.Frameworks.GameServices.Prefabs.Implementation;
using Sources.Frameworks.GameServices.Volumes.Domain.Models.Implementation;
using Sources.Frameworks.UiFramework.AudioSources.Domain.Configs;
using Sources.Frameworks.UiFramework.AudioSources.Domain.Groups;
using Sources.Frameworks.UiFramework.AudioSources.Domain.Groups.Types;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Factories.Implementation;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Factories.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.Spawners.Implementation;
using Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.Spawners.Interfaces;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Implementation.Types;
using Sources.Frameworks.UiFramework.AudioSources.Presentations.Interfaces;
using Sources.Frameworks.UiFramework.Collectors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sources.Frameworks.UiFramework.AudioSources.Infrastructure.Services.AudioService.Implementation
{
    public class AudioService : IAudioService
    {
        private readonly AudioServiceDataBase _audioServiceDataBase;
        private readonly Dictionary<AudioSourceId, IUiAudioSource> _audioSources;
        private readonly Dictionary<AudioGroupId, AudioGroup> _audioGroups;
        private readonly ObjectPool<UiAudioSource> _audioSourcePool;
        private readonly IAudioSourceSpawner _audioSourceSpawner;

        private Volume _volume;
        private CancellationTokenSource _audioCancellationTokenSource;

        public AudioService(
            UiCollector uiCollector,
            AudioServiceDataBase audioServiceDataBase)
        {
            _audioServiceDataBase = audioServiceDataBase ??
                                    throw new ArgumentNullException(nameof(audioServiceDataBase));

            _audioSources = uiCollector.UiAudioSources.ToDictionary(
                uiAudioSource => uiAudioSource.AudioSourceId, uiAudioSource => uiAudioSource);

            _audioGroups = audioServiceDataBase.AudioGroups;
            _audioSourcePool = new ObjectPool<UiAudioSource>();
            _audioSourcePool.SetPoolCount(_audioServiceDataBase.PoolCount);
            IAudioContainerFactory audioContainerFactory = new AudioContainerFactory(
                _audioSourcePool, new ResourcesPrefabLoader());
            _audioSourceSpawner = new AudioSourceSpawner(_audioSourcePool, audioContainerFactory);
        }

        public void Construct(Volume volume) =>
            _volume = volume ?? throw new ArgumentNullException(nameof(volume));

        public void Initialize()
        {
            if (_volume == null)
                throw new NullReferenceException(nameof(_volume));

            _audioCancellationTokenSource = new CancellationTokenSource();
            ClearStates();
            OnVolumeChanged();
            _audioServiceDataBase.Construct(_volume);
            _volume.MusicVolumeChanged += OnVolumeChanged;
        }

        public void Destroy()
        {
            _volume.MusicVolumeChanged -= OnVolumeChanged;
            _audioCancellationTokenSource.Cancel();
            ClearStates();
        }

        public void Play(AudioSourceId id)
        {
            if (_audioSources.ContainsKey(id) == false)
                throw new KeyNotFoundException(id.ToString());

            _audioSources[id].Play();
        }

        public IUiAudioSource Play(AudioGroupId audioGroupId)
        {
            UiAudioSource audioSource = _audioSourceSpawner.Spawn();
            audioSource.SetVolume(_volume.MusicVolume);

            if (_audioGroups.ContainsKey(audioGroupId) == false)
                throw new KeyNotFoundException(audioGroupId.ToString());

            // audioSource.SetClip(_audioGroups[audioGroupId]);
            // audioSource?.PlayAsync(audioSource.Destroy);
            
            return audioSource;
        }

        public async void PlayAsync(AudioGroupId audioGroupId)
        {
            if (_audioGroups.ContainsKey(audioGroupId) == false)
                throw new KeyNotFoundException(audioGroupId.ToString());
            
            IUiAudioSource audioSource = _audioSourceSpawner.Spawn();
            audioSource.SetVolume(_volume.MusicVolume);
            AudioGroup audioGroup = _audioGroups[audioGroupId];

            try
            {
                    if (audioGroup.Type == PlayingType.Loop)
                        await PlayLoop(audioGroupId, audioSource);
                    else if (audioGroup.Type == PlayingType.Default)
                        PlayOneShot(audioGroupId, audioSource);
                    else if (audioGroup.Type == PlayingType.Random)
                        PlayRandom(audioGroupId, audioSource);
            }
            catch (OperationCanceledException)
            {
                audioSource.StopPlayAsync();
            }
        }

        public void Stop(AudioGroupId audioGroupId)
        {
            if (_audioGroups.ContainsKey(audioGroupId) == false)
                throw new KeyNotFoundException(audioGroupId.ToString());

            if (_audioGroups[audioGroupId].IsPlaying == false)
                return;

            _audioGroups[audioGroupId].Stop();
        }

        private void ClearStates()
        {
            foreach (AudioGroup audioGroup in _audioGroups.Values)
            {
                audioGroup.Stop();
                audioGroup.Destroy();
            }
        }

        private void OnVolumeChanged() =>
            ChangeVolume(_volume.MusicVolume);

        private void ChangeVolume(float volume)
        {
            foreach (IUiAudioSource audioSource in _audioSources.Values)
                audioSource.SetVolume(volume);

            foreach (UiAudioSource audioSource in _audioSourcePool.Collection)
                audioSource.SetVolume(volume);
        }

        private async UniTask PlayLoop(AudioGroupId audioGroupId, IUiAudioSource audioSource)
        {
            if (_audioGroups[audioGroupId].IsPlaying)
                throw new InvalidOperationException($"Group {audioGroupId} is already playing");
            
            AudioGroup audioGroup = _audioGroups[audioGroupId];
            audioGroup.Play();
            
            while (_audioCancellationTokenSource.Token.IsCancellationRequested == false &&
                   _audioGroups[audioGroupId].IsPlaying)
            {

                foreach (AudioClip audioClip in _audioGroups[audioGroupId].AudioClips)
                {
                    audioSource.SetClip(audioClip);
                    _audioGroups[audioGroupId].SetCurrentClip(audioClip);
                    await audioSource.PlayAsync(audioGroup: _audioGroups[audioGroupId]);
                }
            }
        }

        private void PlayRandom(AudioGroupId audioGroupId, IUiAudioSource audioSource)
        {
            if (_audioGroups[audioGroupId].AudioClips.Count <= 0)
                throw new ArgumentOutOfRangeException();
            
            AudioGroup audioGroup = _audioGroups[audioGroupId];
            audioGroup.Play();
            
            int randomIndex = Random.Range(0, _audioGroups[audioGroupId].AudioClips.Count);
            AudioClip clip = _audioGroups[audioGroupId].AudioClips[randomIndex];
            
            audioSource.SetClip(clip);
            audioSource?.PlayAsync(audioSource.Destroy);
        }

        private void PlayOneShot(AudioGroupId audioGroupId, IUiAudioSource audioSource)
        {
            if (_audioGroups[audioGroupId].AudioClips.Count <= 0)
                throw new ArgumentOutOfRangeException();
            
            AudioGroup audioGroup = _audioGroups[audioGroupId];
            audioGroup.Play();
            
            audioSource.SetClip(_audioGroups[audioGroupId].AudioClips.First());
            audioSource?.PlayAsync(audioSource.Destroy);
        }
    }
}