using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HoldTheHill.Core.Audio
{
    /// <summary>
    /// Central audio manager for Hold the Hill.
    /// Manages pooled SFX voices (zero runtime GC allocation), cross-faded music channels,
    /// and AudioMixer volume parameters. Hooks into <see cref="SoundEffectSO.PlaybackHandler"/>
    /// so any SoundEffectSO asset routes through this system automatically.
    /// </summary>
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        public const string MasterVolumeParam = "MasterVolume";
        public const string SfxVolumeParam = "SfxVolume";
        public const string MusicVolumeParam = "MusicVolume";

        private static AudioManager s_current;

        /// <summary>The active AudioManager instance, or null if none is in the scene.</summary>
        public static AudioManager Current => s_current;

        /// <summary>Alias for Current to support singleton-style access.</summary>
        public static AudioManager Instance => s_current;

        [Header("Mixer Routing")]
        [Tooltip("The master AudioMixer asset for the project.")]
        [SerializeField] private AudioMixer _mixer;

        [Tooltip("Default mixer group for sound effects.")]
        [SerializeField] private AudioMixerGroup _sfxGroup;

        [Tooltip("Default mixer group for music and background ambience.")]
        [SerializeField] private AudioMixerGroup _musicGroup;

        [Header("SFX Pool Settings")]
        [Tooltip("Number of pre-warmed AudioSources for sound effects.")]
        [SerializeField, Min(4)] private int _sfxPoolSize = 16;

        [Header("Music Settings")]
        [Tooltip("Optional background music to start on enable.")]
        [SerializeField] private AudioClip _defaultMusic;

        [Tooltip("If true, starts default music when enabled.")]
        [SerializeField] private bool _playMusicOnStart;

        // Pooled SFX sources
        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
        private Transform _poolRoot;
        private bool _poolInitialized;

        // Dual music sources for smooth cross-fading
        private AudioSource _musicSourceA;
        private AudioSource _musicSourceB;
        private bool _isUsingSourceA = true;
        private Coroutine _musicFadeCoroutine;

        /// <summary>Total capacity of the SFX voice pool.</summary>
        public int PoolSize => _sfxPool.Count;

        /// <summary>Number of SFX voices currently playing.</summary>
        public int ActiveVoiceCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _sfxPool.Count; i++)
                {
                    if (_sfxPool[i] != null && _sfxPool[i].isPlaying)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>The AudioSource currently playing music.</summary>
        public AudioSource CurrentMusicSource => _isUsingSourceA ? _musicSourceA : _musicSourceB;

        /// <summary>Configured Master/SFX AudioMixer.</summary>
        public AudioMixer Mixer
        {
            get => _mixer;
            set => _mixer = value;
        }

        /// <summary>Default AudioMixerGroup for SFX.</summary>
        public AudioMixerGroup SfxGroup
        {
            get => _sfxGroup;
            set => _sfxGroup = value;
        }

        /// <summary>Default AudioMixerGroup for Music.</summary>
        public AudioMixerGroup MusicGroup
        {
            get => _musicGroup;
            set => _musicGroup = value;
        }

        private void Awake()
        {
            if (s_current != null && s_current != this)
            {
                Destroy(gameObject);
                return;
            }
            s_current = this;

            EnsurePoolInitialized();
            InitializeMusicSources();
        }

        private void OnEnable()
        {
            if (s_current == null)
            {
                s_current = this;
            }

            SoundEffectSO.PlaybackHandler = HandlePlaybackRequest;

            if (_playMusicOnStart && _defaultMusic != null)
            {
                PlayMusic(_defaultMusic, fadeDuration: 0f);
            }
        }

        private void OnDisable()
        {
            if (SoundEffectSO.PlaybackHandler == HandlePlaybackRequest)
            {
                SoundEffectSO.PlaybackHandler = null;
            }
        }

        private void OnDestroy()
        {
            if (s_current == this)
            {
                s_current = null;
            }
        }

        /// <summary>
        /// Plays a sound effect asset at the given world position using an idle pooled voice.
        /// </summary>
        public AudioSource PlayOneShot(SoundEffectSO sfx, Vector3 position = default, float spatialBlend = 0f)
        {
            if (sfx == null)
            {
                return null;
            }

            AudioClip clip = sfx.SelectClip();
            if (clip == null)
            {
                return null;
            }

            float pitch = sfx.GetRandomPitch();
            float volume = sfx.Volume;
            AudioMixerGroup group = sfx.MixerGroup != null ? sfx.MixerGroup : _sfxGroup;

            return PlayOneShot(clip, volume, pitch, position, spatialBlend, group);
        }

        /// <summary>
        /// Plays a raw AudioClip directly with volume, pitch, position, and mixer routing.
        /// </summary>
        public AudioSource PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3 position = default, float spatialBlend = 0f, AudioMixerGroup group = null)
        {
            if (clip == null)
            {
                return null;
            }

            EnsurePoolInitialized();
            AudioSource source = GetAvailableSource();
            if (source == null)
            {
                return null;
            }

            source.transform.position = position;
            source.clip = clip;
            source.outputAudioMixerGroup = group != null ? group : _sfxGroup;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = pitch;
            source.spatialBlend = Mathf.Clamp01(spatialBlend);
            source.loop = false;
            source.Play();

            return source;
        }

        /// <summary>
        /// Cross-fades into a new background music track.
        /// </summary>
        public void PlayMusic(AudioClip clip, float fadeDuration = 1f, bool loop = true)
        {
            if (clip == null)
            {
                StopMusic(fadeDuration);
                return;
            }

            InitializeMusicSources();

            AudioSource activeSource = _isUsingSourceA ? _musicSourceA : _musicSourceB;
            AudioSource nextSource = _isUsingSourceA ? _musicSourceB : _musicSourceA;

            if (activeSource.isPlaying && activeSource.clip == clip)
            {
                return;
            }

            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }

            nextSource.clip = clip;
            nextSource.loop = loop;
            nextSource.outputAudioMixerGroup = _musicGroup;

            if (fadeDuration <= 0f || !isActiveAndEnabled)
            {
                activeSource.Stop();
                activeSource.volume = 0f;
                nextSource.volume = 1f;
                nextSource.Play();
                _isUsingSourceA = !_isUsingSourceA;
            }
            else
            {
                _musicFadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(activeSource, nextSource, fadeDuration));
                _isUsingSourceA = !_isUsingSourceA;
            }
        }

        /// <summary>
        /// Fades out and stops the currently playing music track.
        /// </summary>
        public void StopMusic(float fadeDuration = 1f)
        {
            if (_musicSourceA == null && _musicSourceB == null)
            {
                return;
            }

            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }

            AudioSource activeSource = _isUsingSourceA ? _musicSourceA : _musicSourceB;

            if (fadeDuration <= 0f || !isActiveAndEnabled || !activeSource.isPlaying)
            {
                if (_musicSourceA != null) { _musicSourceA.Stop(); _musicSourceA.volume = 0f; }
                if (_musicSourceB != null) { _musicSourceB.Stop(); _musicSourceB.volume = 0f; }
            }
            else
            {
                _musicFadeCoroutine = StartCoroutine(FadeOutMusicCoroutine(activeSource, fadeDuration));
            }
        }

        // --- AudioMixer UI Volume Controls ---

        /// <summary>Sets Master volume from a linear 0..1 slider.</summary>
        public void SetMasterVolume(float linear01) => SetVolume(MasterVolumeParam, linear01);

        /// <summary>Sets SFX bus volume from a linear 0..1 slider.</summary>
        public void SetSfxVolume(float linear01) => SetVolume(SfxVolumeParam, linear01);

        /// <summary>Sets Music bus volume from a linear 0..1 slider.</summary>
        public void SetMusicVolume(float linear01) => SetVolume(MusicVolumeParam, linear01);

        /// <summary>
        /// Sets an exposed AudioMixer parameter using logarithmic decibel conversion.
        /// </summary>
        public void SetVolume(string parameterName, float linear01)
        {
            if (_mixer == null || string.IsNullOrEmpty(parameterName))
            {
                return;
            }

            float clamped = Mathf.Clamp01(linear01);
            float db = clamped > 0.0001f ? Mathf.Log10(clamped) * 20f : -80f;
            _mixer.SetFloat(parameterName, db);
        }

        private void HandlePlaybackRequest(SoundEffectSO sfx, Vector3 position, float spatialBlend)
        {
            PlayOneShot(sfx, position, spatialBlend);
        }

        private void EnsurePoolInitialized()
        {
            if (_poolInitialized && _sfxPool.Count > 0)
            {
                return;
            }
            _poolInitialized = true;

            if (_poolRoot == null)
            {
                var rootObj = new GameObject("SFX_VoicePool");
                rootObj.transform.SetParent(transform);
                _poolRoot = rootObj.transform;
            }

            int needed = Mathf.Max(4, _sfxPoolSize);
            while (_sfxPool.Count < needed)
            {
                var voiceObj = new GameObject($"Voice_{_sfxPool.Count:00}");
                voiceObj.transform.SetParent(_poolRoot);
                var source = voiceObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.outputAudioMixerGroup = _sfxGroup;
                _sfxPool.Add(source);
            }
        }

        private void InitializeMusicSources()
        {
            if (_musicSourceA == null)
            {
                _musicSourceA = gameObject.AddComponent<AudioSource>();
                _musicSourceA.loop = true;
                _musicSourceA.playOnAwake = false;
                _musicSourceA.outputAudioMixerGroup = _musicGroup;
            }

            if (_musicSourceB == null)
            {
                _musicSourceB = gameObject.AddComponent<AudioSource>();
                _musicSourceB.loop = true;
                _musicSourceB.playOnAwake = false;
                _musicSourceB.outputAudioMixerGroup = _musicGroup;
            }
        }

        private AudioSource GetAvailableSource()
        {
            // 1. Search for an idle source
            for (int i = 0; i < _sfxPool.Count; i++)
            {
                if (_sfxPool[i] != null && !_sfxPool[i].isPlaying)
                {
                    return _sfxPool[i];
                }
            }

            // 2. Voice stealing: take the source closest to ending
            AudioSource bestCandidate = _sfxPool[0];
            float maxProgress = -1f;

            for (int i = 0; i < _sfxPool.Count; i++)
            {
                AudioSource candidate = _sfxPool[i];
                if (candidate == null) continue;

                if (candidate.clip != null && candidate.clip.length > 0f)
                {
                    float progress = candidate.time / candidate.clip.length;
                    if (progress > maxProgress)
                    {
                        maxProgress = progress;
                        bestCandidate = candidate;
                    }
                }
                else
                {
                    return candidate;
                }
            }

            if (bestCandidate != null)
            {
                bestCandidate.Stop();
            }
            return bestCandidate;
        }

        private IEnumerator CrossfadeMusicCoroutine(AudioSource fadeOut, AudioSource fadeIn, float duration)
        {
            fadeIn.volume = 0f;
            fadeIn.Play();

            float startOutVol = fadeOut.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                fadeOut.volume = Mathf.Lerp(startOutVol, 0f, t);
                fadeIn.volume = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            fadeOut.Stop();
            fadeOut.volume = 0f;
            fadeIn.volume = 1f;
            _musicFadeCoroutine = null;
        }

        private IEnumerator FadeOutMusicCoroutine(AudioSource source, float duration)
        {
            float startVol = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(startVol, 0f, t);
                yield return null;
            }

            source.Stop();
            source.volume = 0f;
            _musicFadeCoroutine = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_current = null;
        }
    }
}

