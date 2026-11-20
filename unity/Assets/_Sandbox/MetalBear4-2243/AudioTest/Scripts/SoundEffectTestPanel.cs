using System;
using HoldTheHill.Core.Audio;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoldTheHill.Sandbox.AudioTest
{
    /// <summary>
    /// Collapsible uGUI test harness for the sound effect system in the sandbox level.
    /// Press F2 or click the top-bar button to toggle the panel without blocking gameplay.
    /// Uses a uGUI Canvas so clicks are ignored by TowerPlacer and SilverfishClickReader.
    /// </summary>
    [DisallowMultipleComponent]
    public class SoundEffectTestPanel : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private LevelRun _run;
        [SerializeField] private LevelAudioDirector _director;
        [SerializeField] private EnemyPath _path;

        [Header("Macro SoundEffectSO Assets")]
        [SerializeField] private SoundEffectSO _waveStartedSfx;
        [SerializeField] private SoundEffectSO _waveCompletedSfx;
        [SerializeField] private SoundEffectSO _queenDamagedSfx;
        [SerializeField] private SoundEffectSO _victorySfx;
        [SerializeField] private SoundEffectSO _defeatSfx;
        [SerializeField] private SoundEffectSO _bossAppearedSfx;
        [SerializeField] private SoundEffectSO _centipedeSplitSfx;
        [SerializeField] private SoundEffectSO _towerPlacedSfx;

        [Header("Music Crossfade Test Tracks")]
        [SerializeField] private AudioClip _calmMusic;
        [SerializeField] private AudioClip _intenseMusic;

        [Header("Live Gameplay Test Prefabs")]
        [SerializeField] private BasicEnemy _centipedePrefab;
        [SerializeField] private BasicEnemy _silverfishPrefab;
        [SerializeField] private BasicEnemy _queenMantisPrefab;

        [Header("Panel Settings")]
        [SerializeField] private bool _startExpanded = false;

        private GameObject _drawer;
        private TMP_Text _statusText;
        private TMP_Text _lastEventText;
        private string _lastEvent = "Ready. Press F2 or click Audio Test.";
        private bool _playLeftNext = true;

        private void Start()
        {
            BuildUi();
            SetExpanded(_startExpanded);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame && _drawer != null)
            {
                SetExpanded(!_drawer.activeSelf);
            }

            if (_drawer != null && _drawer.activeSelf)
            {
                RefreshStatus();
            }
        }

        private void SetExpanded(bool expanded)
        {
            if (_drawer != null)
            {
                _drawer.SetActive(expanded);
            }

            if (expanded)
            {
                RefreshStatus();
            }
        }

        private void RefreshStatus()
        {
            AudioManager am = AudioManager.Current;
            string voices = am != null ? $"{am.ActiveVoiceCount}/{am.PoolSize}" : "NO MANAGER";
            string handler = SoundEffectSO.PlaybackHandler != null ? "Bound" : "Fallback";
            string dirState = _director != null && _director.BoundRun != null ? "Bound" : "Unbound";
            string musicState = am != null && am.CurrentMusicSource != null && am.CurrentMusicSource.isPlaying
                ? am.CurrentMusicSource.clip.name
                : "Stopped";

            if (_statusText != null)
            {
                _statusText.text = $"Voices: {voices} | Handler: {handler}\nDirector: {dirState} | Music: {musicState}";
            }

            if (_lastEventText != null)
            {
                _lastEventText.text = $"Last: {_lastEvent}";
            }
        }

        private void RecordEvent(string message)
        {
            _lastEvent = message;
            RefreshStatus();
        }

        private void Play2D(SoundEffectSO sfx, string label)
        {
            if (sfx == null)
            {
                RecordEvent($"{label} missing!");
                return;
            }

            bool couldPlay = sfx.CanPlay;
            sfx.Play();
            RecordEvent(couldPlay ? $"Played 2D: {label}" : $"Throttled (cooldown): {label}");
        }

        private void PlaySpatialLeftRight(SoundEffectSO sfx, string label)
        {
            if (sfx == null)
            {
                RecordEvent($"{label} missing!");
                return;
            }

            Vector3 pos = _playLeftNext ? new Vector3(-8f, 0f, 0f) : new Vector3(8f, 0f, 0f);
            string side = _playLeftNext ? "LEFT (-8)" : "RIGHT (+8)";
            _playLeftNext = !_playLeftNext;

            bool couldPlay = sfx.CanPlay;
            sfx.Play(pos);
            RecordEvent(couldPlay ? $"Played 3D {side}: {label}" : $"Throttled: {label}");
        }

        private void RunCooldownSpamTest()
        {
            if (_waveStartedSfx == null)
            {
                RecordEvent("WaveStart SFX missing!");
                return;
            }

            _waveStartedSfx.ResetCooldown();
            int allowed = 0;
            for (int i = 0; i < 5; i++)
            {
                if (_waveStartedSfx.CanPlay)
                {
                    allowed++;
                }
                _waveStartedSfx.Play();
            }

            RecordEvent($"Cooldown spam (x5): {allowed} played, {5 - allowed} blocked");
        }

        private void ResetAllCooldowns()
        {
            SoundEffectSO[] all =
            {
                _waveStartedSfx, _waveCompletedSfx, _queenDamagedSfx, _victorySfx,
                _defeatSfx, _bossAppearedSfx, _centipedeSplitSfx, _towerPlacedSfx
            };

            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null)
                {
                    all[i].ResetCooldown();
                }
            }

            RecordEvent("Reset cooldowns on all 8 SFX");
        }

        private void BurstTwentyVoices()
        {
            AudioManager am = AudioManager.Current;
            if (am == null || _towerPlacedSfx == null || _towerPlacedSfx.Clips == null || _towerPlacedSfx.Clips.Length == 0)
            {
                RecordEvent("Need AudioManager + clip for burst");
                return;
            }

            AudioClip clip = _towerPlacedSfx.Clips[0];
            for (int i = 0; i < 20; i++)
            {
                float pitch = 0.75f + i * 0.03f;
                am.PlayOneShot(clip, volume: 0.35f, pitch: pitch, position: Vector3.zero, spatialBlend: 0f);
            }

            RecordEvent($"Burst 20 voices (pool={am.PoolSize}, active={am.ActiveVoiceCount})");
        }

        private void PlayMusicTrack(AudioClip clip, string label)
        {
            if (AudioManager.Current == null || clip == null)
            {
                RecordEvent($"Cannot play {label}");
                return;
            }

            AudioManager.Current.PlayMusic(clip, fadeDuration: 1f, loop: true);
            RecordEvent($"Crossfading music -> {label}");
        }

        private void StopMusicTrack()
        {
            if (AudioManager.Current == null)
            {
                return;
            }

            AudioManager.Current.StopMusic(fadeDuration: 0.8f);
            RecordEvent("Fading out music (0.8s)");
        }

        private void DamageQueen()
        {
            LevelRun run = _run != null ? _run : LevelRun.Current;
            if (run == null || run.Queen == null)
            {
                RecordEvent("No Queen found!");
                return;
            }

            run.Queen.TakeDamage(10);
            RecordEvent($"Damaged Queen -10 (HP: {run.Queen.CurrentHealth}/{run.Queen.MaxHealth})");
        }

        private void SpawnTestEnemy(BasicEnemy prefab, string label)
        {
            if (prefab == null || _path == null)
            {
                RecordEvent($"Missing prefab or path for {label}");
                return;
            }

            Vector3 spawnPos = _path.Waypoints.Count > 0 ? _path.Waypoints[0] : Vector3.zero;
            BasicEnemy enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(enemy.gameObject, gameObject.scene);
            enemy.SetPath(_path, teleportToStart: true);
            RecordEvent($"Spawned {label} on path");
        }

        private void GrantHoneydew()
        {
            LevelRun run = _run != null ? _run : LevelRun.Current;
            if (run == null || run.Wallet == null)
            {
                RecordEvent("No Wallet found!");
                return;
            }

            run.Wallet.Honeydew.Earn(500);
            RecordEvent($"+500 Honeydew (total: {run.Wallet.Honeydew.Amount})");
        }

        // ---- Runtime uGUI Builder ------------------------------------------------------------

        private void BuildUi()
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer < 0) uiLayer = 0;

            var canvasGo = new GameObject("AudioTestCanvas", typeof(RectTransform));
            canvasGo.layer = uiLayer;
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // Toggle pill in top-center-left gap (clear of Honeydew, Controls Help, Wave, and Queen Bar).
            Button toggle = CreateButton(canvasGo.transform, "ToggleAudioTest", "Audio Test (F2)",
                new Color(0.16f, 0.28f, 0.36f, 0.95f), Color.white, 18, uiLayer);
            Anchor((RectTransform)toggle.transform, new Vector2(0.27f, 1f), new Vector2(0f, -20f), new Vector2(210f, 42f));
            toggle.onClick.AddListener(() => SetExpanded(!_drawer.activeSelf));

            // Collapsible test drawer pinned along the left side.
            var drawerImg = CreateImage(canvasGo.transform, "Drawer", new Color(0.08f, 0.1f, 0.14f, 0.92f), uiLayer);
            _drawer = drawerImg.gameObject;
            Anchor(drawerImg.rectTransform, new Vector2(0f, 0.5f), new Vector2(18f, -20f), new Vector2(360f, 650f));

            float y = -14f;

            _statusText = CreateLabel(_drawer.transform, "Status", "", 15, Color.cyan, uiLayer);
            Anchor(_statusText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, y), new Vector2(336f, 40f));
            y -= 44f;

            _lastEventText = CreateLabel(_drawer.transform, "LastEvent", "", 14, new Color(0.85f, 0.95f, 0.7f), uiLayer);
            Anchor(_lastEventText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, y), new Vector2(336f, 24f));
            y -= 30f;

            y = AddSectionHeader("Macro SoundEffectSOs", y, uiLayer);
            y = AddButtonRow(y, uiLayer,
                ("Wave Start", () => Play2D(_waveStartedSfx, "WaveStart")),
                ("Wave Clear", () => Play2D(_waveCompletedSfx, "WaveClear")));
            y = AddButtonRow(y, uiLayer,
                ("Queen Hurt", () => Play2D(_queenDamagedSfx, "QueenDamaged")),
                ("Boss Appear", () => Play2D(_bossAppearedSfx, "BossAppeared")));
            y = AddButtonRow(y, uiLayer,
                ("Victory", () => Play2D(_victorySfx, "Victory")),
                ("Defeat", () => Play2D(_defeatSfx, "Defeat")));
            y = AddButtonRow(y, uiLayer,
                ("Split (3D L/R)", () => PlaySpatialLeftRight(_centipedeSplitSfx, "CentipedeSplit")),
                ("Place (3D L/R)", () => PlaySpatialLeftRight(_towerPlacedSfx, "TowerPlaced")));

            y -= 6f;
            y = AddSectionHeader("Pooling & Cooldown Tests", y, uiLayer);
            y = AddButtonRow(y, uiLayer,
                ("Spam x5 (Cooldown)", RunCooldownSpamTest),
                ("Reset Cooldowns", ResetAllCooldowns));
            y = AddSingleButton(y, uiLayer, "Burst 20 Voices (Test Voice Stealing)", BurstTwentyVoices);

            y -= 6f;
            y = AddSectionHeader("AudioManager Music Crossfade", y, uiLayer);
            y = AddButtonRow(y, uiLayer,
                ("Play Calm", () => PlayMusicTrack(_calmMusic, "Calm")),
                ("Crossfade Intense", () => PlayMusicTrack(_intenseMusic, "Intense")));
            y = AddSingleButton(y, uiLayer, "Stop Music (0.8s Fade)", StopMusicTrack);

            y -= 6f;
            y = AddSectionHeader("Live Gameplay Triggers", y, uiLayer);
            y = AddButtonRow(y, uiLayer,
                ("Hurt Queen (-10)", DamageQueen),
                ("+500 Honeydew", GrantHoneydew));
            y = AddButtonRow(y, uiLayer,
                ("Spawn Centipede", () => SpawnTestEnemy(_centipedePrefab, "Centipede")),
                ("Spawn Silverfish", () => SpawnTestEnemy(_silverfishPrefab, "Silverfish")));
            AddSingleButton(y, uiLayer, "Spawn Queen Mantis Boss", () => SpawnTestEnemy(_queenMantisPrefab, "Queen Mantis"));
        }

        private float AddSectionHeader(string title, float y, int layer)
        {
            TMP_Text header = CreateLabel(_drawer.transform, title, title, 15, new Color(1f, 0.85f, 0.45f), layer);
            header.fontStyle = FontStyles.Bold;
            Anchor(header.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, y), new Vector2(336f, 22f));
            return y - 26f;
        }

        private float AddButtonRow(float y, int layer, (string label, Action onClick) left, (string label, Action onClick) right)
        {
            Button b1 = CreateButton(_drawer.transform, left.label, left.label, new Color(0.22f, 0.27f, 0.34f), Color.white, 14, layer);
            Anchor((RectTransform)b1.transform, new Vector2(0.5f, 1f), new Vector2(-86f, y), new Vector2(164f, 32f));
            b1.onClick.AddListener(() => left.onClick?.Invoke());

            Button b2 = CreateButton(_drawer.transform, right.label, right.label, new Color(0.22f, 0.27f, 0.34f), Color.white, 14, layer);
            Anchor((RectTransform)b2.transform, new Vector2(0.5f, 1f), new Vector2(86f, y), new Vector2(164f, 32f));
            b2.onClick.AddListener(() => right.onClick?.Invoke());

            return y - 38f;
        }

        private float AddSingleButton(float y, int layer, string label, Action onClick)
        {
            Button b = CreateButton(_drawer.transform, label, label, new Color(0.22f, 0.27f, 0.34f), Color.white, 14, layer);
            Anchor((RectTransform)b.transform, new Vector2(0.5f, 1f), new Vector2(0f, y), new Vector2(336f, 32f));
            b.onClick.AddListener(() => onClick?.Invoke());
            return y - 38f;
        }

        private static Image CreateImage(Transform parent, string name, Color color, int layer)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = layer;
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private static TMP_Text CreateLabel(Transform parent, string name, string content, int fontSize, Color color, int layer)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = layer;
            go.transform.SetParent(parent, false);
            var txt = go.AddComponent<TextMeshProUGUI>();
            txt.text = content;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = TextAlignmentOptions.MidlineLeft;
            txt.raycastTarget = false;
            return txt;
        }

        private static Button CreateButton(Transform parent, string name, string label, Color bgColor, Color textColor, int fontSize, int layer)
        {
            Image bg = CreateImage(parent, name, bgColor, layer);
            var btn = bg.gameObject.AddComponent<Button>();
            btn.targetGraphic = bg;

            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            btn.colors = colors;

            TMP_Text txt = CreateLabel(bg.transform, "Text", label, fontSize, textColor, layer);
            txt.alignment = TextAlignmentOptions.Center;
            RectTransform tr = txt.rectTransform;
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(4f, 2f);
            tr.offsetMax = new Vector2(-4f, -2f);
            return btn;
        }

        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
        }
    }
}
