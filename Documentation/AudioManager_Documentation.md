# Technical Documentation: AudioManager.cs (Audio Playback & Mixer Service)

**Target Unity Version**: Unity 6 (`6000.6.0f1`)  
**Context**: 2D Hybrid Tower Defense Game (*Hold the Hill*)  
**File Location**: `unity/Assets/_Game/Core/Audio/AudioManager.cs`  
**Namespace**: `HoldTheHill.Core.Audio`  
**Assembly**: `HoldTheHill.Runtime`  
**Test Suite**: `unity/Assets/_Game/Tests/EditMode/AudioManagerTests.cs`  
**Companion Systems**:
- `SoundEffectSO.cs` (`_Game/Core/Audio/`) — ScriptableObject audio event cards
- `LevelAudioDirector.cs` (`_Game/Features/GameFlow/Scripts/`) — macro run flow listener
- `SceneLoader.cs` (`_Game/Core/Bootstrap/`) — persistent scene bootstrap host

---

## 1. Overview & Architecture

`AudioManager` is the centralized playback engine and audio mixer controller for *Hold the Hill*. It provides a robust, zero-garbage-collection voice pool for sound effects, dual-source music cross-fading, and logarithmic volume attenuation for settings menus.

Rather than instantiating GameObjects or creating local `AudioSource` components on every projectile or enemy, all sound effects are dispatched through `AudioManager`'s pre-warmed voice pool.

```mermaid
flowchart TD
    subgraph AudioCallers["Sound Triggers"]
        SO["SoundEffectSO.Play()"]
        Direct["Direct C# API Calls\n(PlayOneShot, PlayMusic)"]
    end

    subgraph Manager["AudioManager (Scenes/Bootstrap)"]
        Handler["SoundEffectSO.PlaybackHandler Bridge"]
        PoolManager["Voice Pool Controller"]
        MusicManager["Dual-Channel Music Crossfader"]
        MixerManager["AudioMixer Parameter Converter"]
    end

    subgraph Pool["SFX Voice Pool (Hierarchy)"]
        Voice0["Voice 00 (AudioSource)"]
        Voice1["Voice 01 (AudioSource)"]
        VoiceN["Voice N.. (AudioSource)"]
        Stealing{"All Playing?\n(Voice Stealing)"}
    end

    subgraph Output["Unity Audio Pipeline"]
        Mixer["AudioMixer (MainMixer)"]
        BusSFX["SFX Bus"]
        BusMusic["Music Bus"]
        BusMaster["Master Bus"]
    end

    SO -->|Invokes Static Delegate| Handler
    Handler --> PoolManager
    Direct --> PoolManager
    Direct --> MusicManager
    Direct --> MixerManager

    PoolManager --> Voice0
    PoolManager --> Voice1
    PoolManager --> VoiceN
    PoolManager --> Stealing
    Stealing -->|Reclaims oldest voice| Voice0

    Voice0 --> BusSFX
    Voice1 --> BusSFX
    VoiceN --> BusSFX
    MusicManager --> BusMusic
    BusSFX --> BusMaster
    BusMusic --> BusMaster
    BusMaster --> AudioOutput["Speakers / AudioListener"]
```

### Architectural Highlights

1. **Automatic `SoundEffectSO` Binding:**
   In `OnEnable()`, `AudioManager` automatically registers its `PlayOneShot` method with `SoundEffectSO.PlaybackHandler`. Any call to `soundEffect.Play()` anywhere in the codebase immediately routes into the pool without requiring direct singleton calls.
2. **Pre-Warmed AudioSource Pooling (Zero GC):**
   Pre-allocates an array of `AudioSource` child components (default 16 voices). When a sound is triggered, an idle voice is allocated and recycled without heap allocation or GameObject instantiation.
3. **Voice Stealing Algorithm:**
   If all voices in the pool are actively playing during an intense combat wave, the manager identifies the voice with the highest normalized playback progress ($\frac{\text{time}}{\text{clip.length}}$), stops it, and repurposes it for the newest request. This ensures high-priority new sounds are never dropped.
4. **Dual-Channel Music Crossfading:**
   Maintains two dedicated `AudioSource` channels (`_musicSourceA` and `_musicSourceB`). Transitioning tracks fades out the active source while fading in the target source over a configurable duration using unscaled delta time (functional even when game simulation is paused).
5. **Logarithmic AudioMixer Decibel Math & Persistence:**
   Exposes volume slider methods (`SetMasterVolume`, `SetSfxVolume`, `SetMusicVolume`) that translate linear UI slider values ($0.0$ to $1.0$) into logarithmic decibels via `LinearToDecibels`:
   $$\text{dB} = 20 \times \log_{10}(\max(0.0001, \text{sliderValue}))$$
   Values at or below $0.0001$ map to $-80\text{ dB}$. Player settings are stored independently from `_musicVolume` (the base mix trim on music `AudioSource`s) so `MainMixer.mixer` (`Master` $\rightarrow$ `SFX`, `Music`) does not double-attenuate music. Preferences load from `PlayerPrefs` in `Awake()` and apply to the mixer in `Start()` once Unity's DSP graph is initialized.

---

## 2. Inspector Configuration & Fields

```mermaid
classDiagram
    class AudioManager {
        -AudioMixer _mixer
        -AudioMixerGroup _sfxGroup
        -AudioMixerGroup _musicGroup
        -int _sfxPoolSize
        -float _musicVolume
        -AudioClip _defaultMusic
        -bool _playMusicOnStart
        -bool _loopMusic
        +PoolSize int
        +ActiveVoiceCount int
        +CurrentMusicSource AudioSource
        +VolumesChanged Action
        +PlayOneShot(SoundEffectSO, Vector3, float) AudioSource
        +PlayOneShot(AudioClip, float, float, Vector3, float, AudioMixerGroup) AudioSource
        +PlayMusic(AudioClip, float, bool) void
        +StopMusic(float) void
        +SetMasterVolume(float) void
        +SetSfxVolume(float) void
        +SetMusicVolume(float) void
        +SetVolume(string, float) void
        +GetMasterVolume() float
        +GetSfxVolume() float
        +GetMusicVolume() float
        +LoadSavedVolumes() void
        +ApplyAllVolumesToMixer() void
        +ResetVolumesToDefaults() void
        +SaveVolumeSettings() void
        +LinearToDecibels(float) float
        +DecibelsToLinear(float) float
    }
```

| Field | Type | Default | Attributes | Description |
| :--- | :--- | :--- | :--- | :--- |
| `_mixer` | `AudioMixer` | `null` | `[SerializeField]` | The master `AudioMixer` asset (`Assets/_Game/Shared/Audio/MainMixer.mixer`) containing exposed volume parameters. |
| `_sfxGroup` | `AudioMixerGroup` | `null` | `[SerializeField]` | The default mixer bus routing for all pooled SFX voices (including UI sounds). |
| `_musicGroup` | `AudioMixerGroup` | `null` | `[SerializeField]` | The mixer bus routing for background music channels A and B. |
| `_sfxPoolSize` | `int` | `16` | `[SerializeField]`, `[Min(4)]` | Total pre-warmed `AudioSource` voices created in the child pool. |
| `_musicVolume` | `float` | `0.5` | `[SerializeField]`, `[Range(0f, 1f)]` | Base mix trim for background music `AudioSource`s (preserved independently from `MusicVolume`). |
| `_defaultMusic` | `AudioClip` | `null` | `[SerializeField]` | Background track to trigger when the manager enables. |
| `_playMusicOnStart`| `bool` | `false`| `[SerializeField]` | Whether to automatically play `_defaultMusic` on `OnEnable()`. |
| `_loopMusic` | `bool` | `true` | `[SerializeField]` | Whether background music loops indefinitely. |

---

## 3. Runtime API Reference

### Accessors
* `AudioManager.Current`: Returns the active `AudioManager` in the scene (following the project's `SceneLoader.Current` pattern).
* `AudioManager.Instance`: Alias for `Current` for singleton compatibility.

### Playback Methods

#### `AudioSource PlayOneShot(SoundEffectSO sfx, Vector3 position = default, float spatialBlend = 0f)`
Pulls an available voice from the pool, extracts clip, pitch, volume, and mixer parameters from the `SoundEffectSO`, and plays the sound.
```csharp
[SerializeField] private SoundEffectSO _shootSfx;

// Direct invocation (or via _shootSfx.Play())
AudioManager.Current.PlayOneShot(_shootSfx, transform.position);
```

#### `AudioSource PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3 position = default, float spatialBlend = 0f, AudioMixerGroup group = null)`
Direct low-level overload for playing raw `AudioClip` instances without a ScriptableObject asset.

#### `void PlayMusic(AudioClip clip, float fadeDuration = 1f, bool loop = true)`
Cross-fades from the currently playing music track into `clip` over `fadeDuration` seconds. If the track is already playing, the call is ignored.
```csharp
// Cross-fade into combat wave theme over 1.5 seconds:
AudioManager.Current.PlayMusic(_waveCombatTrack, fadeDuration: 1.5f);
```

#### `void StopMusic(float fadeDuration = 1f)`
Fades out and stops any actively playing music track over `fadeDuration` seconds.

### Mixer Volume Controls & Persistence
* `void SetMasterVolume(float linear01)`: Clamps and saves `"HoldTheHill.Audio.MasterVolume"`, updates `"MasterVolume"`, and raises `VolumesChanged`.
* `void SetSfxVolume(float linear01)`: Clamps and saves `"HoldTheHill.Audio.SfxVolume"`, updates `"SfxVolume"`, and raises `VolumesChanged`.
* `void SetMusicVolume(float linear01)`: Clamps and saves `"HoldTheHill.Audio.MusicVolume"`, updates `"MusicVolume"`, preserves `_musicVolume` base trim, and raises `VolumesChanged`.
* `void SetVolume(string parameterName, float linear01)`: Generic method converting $0.0..1.0$ into $-80\text{ dB}..0\text{ dB}$ via `LinearToDecibels`.
* `float GetMasterVolume()`, `float GetSfxVolume()`, `float GetMusicVolume()`: Return the stored linear `0..1` settings.
* `void LoadSavedVolumes()`: Reads persisted `PlayerPrefs` values (called automatically in `Awake()`).
* `void ApplyAllVolumesToMixer()`: Pushes all three stored settings to `_mixer` (called automatically in `Start()` so the DSP graph is ready).
* `void ResetVolumesToDefaults()`: Resets all three settings to `1.0`, writes and flushes `PlayerPrefs`, applies to `_mixer`, and raises `VolumesChanged`.
* `void SaveVolumeSettings()`: Flushes pending `PlayerPrefs` changes to disk (also called on `OnDisable()` and `OnApplicationQuit()`).
* `static float LinearToDecibels(float linear01)` / `static float DecibelsToLinear(float decibels)`: Pure logarithmic conversion helpers (`0.0001f` floor $\leftrightarrow -80\text{ dB}$, `1.0f` $\leftrightarrow 0\text{ dB}$).
* **Null-Mixer Fallback:** When `_mixer` is `null` (such as isolated tests), `PlayOneShot` and `PlayMusic` scale `AudioSource.volume` by the stored Master and channel settings instead.

---

## 4. Scene Setup & Bootstrap Hosting

`AudioManager` is intended to live on a GameObject in **`Scenes/Bootstrap`**, which remains loaded for the entire game session.

```
Scenes/Bootstrap
├── Scene Loader
├── Background Camera
└── Audio Manager
    └── SFX_VoicePool (Auto-generated child)
        ├── Voice_00
        ├── Voice_01
        └── ...
```

* **Domain Reload Protection:** Static instance references are automatically cleared via `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` to ensure Play Mode runs safely.

---

## 5. Unit Testing & Verification

The component and mixer configuration are verified by [AudioManagerTests.cs](../unity/Assets/_Game/Tests/EditMode/AudioManagerTests.cs) and [AudioMixerDataTests.cs](../unity/Assets/_Game/Tests/EditMode/AudioMixerDataTests.cs):

| Test Case | Description |
| :--- | :--- |
| `Awake_EstablishesCurrentInstance` | Verifies `AudioManager.Current` and `AudioManager.Instance` are assigned on Awake. |
| `OnEnable_RegistersPlaybackHandler` | Asserts that `SoundEffectSO.PlaybackHandler` is hooked when the manager enables. |
| `OnDisable_UnregistersPlaybackHandler` | Asserts that `SoundEffectSO.PlaybackHandler` is safely nulled on disable. |
| `PlayOneShot_RawClip_ConfiguresAudioSource` | Confirms pooled voice properties (clip, pitch, volume, position, spatial blend). |
| `PlayOneShot_SoundEffectSO_RoutesThroughPlaybackHandler` | Verifies end-to-end routing from `sfx.Play()` into the pooled voice manager. |
| `PlayMusic_AssignsClipToMusicSource` | Confirms assignment and looping configuration on the music channel. |
| `SetVolume_HandlesNullMixerGracefully` | Verifies null-safety of volume methods when running in headless tests without an AudioMixer. |
| `LinearToDecibels_MapsBoundariesAndMidpointCorrectly` | Checks dB mapping at `0`, `0.0001`, `0.5`, `1`, and out-of-range inputs. |
| `DecibelsToLinear_InvertsLinearToDecibels` | Verifies inverse dB-to-linear clamping and mute floor handling. |
| `SetVolumes_ClampsPersistsAndRaisesEvent` | Confirms clamping, `PlayerPrefs` storage, getters, and `VolumesChanged` firing. |
| `SetMusicVolume_PreservesBaseMusicVolumeTrim` | Confirms slider updates do not overwrite `_musicVolume` base trim. |
| `LoadSavedVolumes_RestoresPersistedPlayerPrefs` | Confirms saved preferences reload cleanly into memory. |
| `ResetVolumesToDefaults_RestoresFullVolumeAndPersists` | Confirms reset restores `1.0` across all channels and persists. |
| `PlayOneShot_NullMixerFallback_ScalesByMasterAndSfxVolumes` | Verifies scalar fallback volume math for SFX when `_mixer` is unassigned. |
| `PlayMusic_NullMixerFallback_ScalesByBaseTrimAndMixerSettings` | Verifies scalar fallback volume math for music when `_mixer` is unassigned. |
| `GroupSetters_PropagateToExistingVoicesAndMusicSources` | Verifies runtime `SfxGroup`/`MusicGroup` assignment updates pooled voices and music channels. |
| `AudioMixerDataTests` (5 tests) | Verifies `MainMixer.mixer` path, 3-group layout (`Master`, `SFX`, `Music`), exposed volume parameters, `Bootstrap.unity` wiring, and shared `SoundEffectSO` routing. |
