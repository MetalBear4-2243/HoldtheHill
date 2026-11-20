using System;
using UnityEngine;
using UnityEngine.Audio;

namespace HoldTheHill.Core.Audio
{
    /// <summary>
    /// ScriptableObject defining an audio effect event.
    /// Encapsulates clip variations, pitch randomization, volume, mixer routing, cooldown throttling,
    /// and spatial blend. Plays via a registered <see cref="PlaybackHandler"/> (e.g. AudioManager)
    /// or directly through an AudioSource.
    /// </summary>
    [CreateAssetMenu(fileName = "sfx_", menuName = "Hold the Hill/Audio/Sound Effect")]
    public class SoundEffectSO : ScriptableObject
    {
        [Header("Audio Clips")]
        [Tooltip("Clips to play. If multiple clips are provided, one is selected at random.")]
        [SerializeField] private AudioClip[] _clips = Array.Empty<AudioClip>();

        [Header("Audio Settings")]
        [Tooltip("AudioMixer group for routing (SFX, UI, Music, etc.).")]
        [SerializeField] private AudioMixerGroup _mixerGroup;

        [Tooltip("Base playback volume (0 to 1).")]
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;

        [Tooltip("Random pitch variance range [min, max], centered around 1.0 (e.g. 0.95 to 1.05).")]
        [SerializeField] private Vector2 _pitchRange = new Vector2(0.95f, 1.05f);

        [Header("Behavior")]
        [Tooltip("Minimum time in seconds between plays to prevent audio stacking / cacophony.")]
        [SerializeField, Min(0f)] private float _cooldown = 0.05f;

        [Tooltip("0 = 2D full stereo (UI, global events), 1 = 3D localized sound.")]
        [SerializeField, Range(0f, 1f)] private float _spatialBlend = 0f;

        // Runtime state (not serialized to disk)
        [NonSerialized] private float _lastPlayTime = -1000f;
        [NonSerialized] private int _lastClipIndex = -1;

        /// <summary>
        /// Global playback delegate registered by the AudioManager.
        /// Signature: (SoundEffectSO sound, Vector3 position, float spatialBlend).
        /// </summary>
        public static Action<SoundEffectSO, Vector3, float> PlaybackHandler;

        /// <summary>The clips assigned to this sound effect.</summary>
        public AudioClip[] Clips
        {
            get => _clips;
            set => _clips = value ?? Array.Empty<AudioClip>();
        }

        /// <summary>The target mixer group (may be null for default output).</summary>
        public AudioMixerGroup MixerGroup
        {
            get => _mixerGroup;
            set => _mixerGroup = value;
        }

        /// <summary>Base volume scale (0 to 1).</summary>
        public float Volume
        {
            get => _volume;
            set => _volume = Mathf.Clamp01(value);
        }

        /// <summary>Randomized pitch range [min, max].</summary>
        public Vector2 PitchRange
        {
            get => _pitchRange;
            set => _pitchRange = value;
        }

        /// <summary>Cooldown in seconds between plays.</summary>
        public float Cooldown
        {
            get => _cooldown;
            set => _cooldown = Mathf.Max(0f, value);
        }

        /// <summary>Spatial blend factor: 0 is 2D stereo, 1 is full 3D spatial.</summary>
        public float SpatialBlend
        {
            get => _spatialBlend;
            set => _spatialBlend = Mathf.Clamp01(value);
        }

        /// <summary>True if the sound has cooled down and can be played.</summary>
        public bool CanPlay => Time.unscaledTime - _lastPlayTime >= _cooldown;

        /// <summary>
        /// Plays the sound effect in 2D stereo (e.g. for UI clicks, wave chimes, global alarms).
        /// </summary>
        public void Play()
        {
            Play(Vector3.zero, is2DOverride: true);
        }

        /// <summary>
        /// Plays the sound effect at a specific world position.
        /// </summary>
        /// <param name="position">World position for spatial playback.</param>
        /// <param name="is2DOverride">If true, forces 2D spatial blend regardless of asset settings.</param>
        public void Play(Vector3 position, bool is2DOverride = false)
        {
            if (_clips == null || _clips.Length == 0)
            {
                return;
            }

            if (!CanPlay)
            {
                return;
            }

            _lastPlayTime = Time.unscaledTime;
            float blend = is2DOverride ? 0f : _spatialBlend;

            if (PlaybackHandler != null)
            {
                PlaybackHandler.Invoke(this, position, blend);
            }
            else
            {
                PlayFallback(position);
            }
        }

        /// <summary>
        /// Plays the sound effect directly through a specific AudioSource component.
        /// Useful for testing, custom emitters, or isolated objects.
        /// </summary>
        /// <param name="source">AudioSource to configure and play on.</param>
        public void Play(AudioSource source)
        {
            if (source == null || _clips == null || _clips.Length == 0)
            {
                return;
            }

            if (!CanPlay)
            {
                return;
            }

            _lastPlayTime = Time.unscaledTime;

            AudioClip clip = SelectClip();
            if (clip == null)
            {
                return;
            }

            source.clip = clip;
            source.outputAudioMixerGroup = _mixerGroup;
            source.volume = _volume;
            source.pitch = GetRandomPitch();
            source.spatialBlend = _spatialBlend;
            source.Play();
        }

        /// <summary>
        /// Selects an AudioClip from the assigned list, avoiding playing the same clip
        /// twice in a row when multiple clips are available.
        /// </summary>
        public AudioClip SelectClip()
        {
            if (_clips == null || _clips.Length == 0)
            {
                return null;
            }

            if (_clips.Length == 1)
            {
                return _clips[0];
            }

            int index;
            int attempts = 0;
            do
            {
                index = UnityEngine.Random.Range(0, _clips.Length);
                attempts++;
            } while (index == _lastClipIndex && attempts < 10);

            _lastClipIndex = index;
            return _clips[index];
        }

        /// <summary>
        /// Computes a randomized pitch within <see cref="_pitchRange"/>.
        /// </summary>
        public float GetRandomPitch()
        {
            if (_pitchRange.x >= _pitchRange.y)
            {
                return _pitchRange.x;
            }

            return UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y);
        }

        /// <summary>
        /// Resets the internal cooldown and clip history (useful for tests).
        /// </summary>
        public void ResetCooldown()
        {
            _lastPlayTime = -1000f;
            _lastClipIndex = -1;
        }

        private void PlayFallback(Vector3 position)
        {
            AudioClip clip = SelectClip();
            if (clip == null)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(clip, position, _volume);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            PlaybackHandler = null;
        }
    }
}

