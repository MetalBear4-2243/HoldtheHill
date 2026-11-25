using System.Collections.Generic;
using HoldTheHill.Core;
using HoldTheHill.Core.Diagnostics;
using HoldTheHill.Core.Save;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.DebugTools
{
    /// <summary>
    /// A play-test panel for the Editor and development builds; a release build turns it off.
    /// The backquote key (`) opens it: frame rate and run numbers, buttons for the dev keys'
    /// actions plus win, lose, restart, home, each level and resetting the save file, and the
    /// last log lines with errors in red. While closed it only shows a small note after an error.
    /// Sits next to <see cref="DevKeys"/> in the UI scene and uses its actions.
    /// </summary>
    /// <remarks>
    /// Drawn with IMGUI so it needs no prefab or art; it allocates while open, which is fine for a
    /// dev tool. Its clicks also reach the map underneath, so close it before building.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DevKeys))]
    public class DevConsole : MonoBehaviour
    {
        [Tooltip("How many log lines the panel shows at most, newest at the bottom; fewer in a short window.")]
        [SerializeField, Range(5, 40)] private int _logLines = 14;

        private readonly List<LogLine> _lines = new List<LogLine>();
        private DevKeys _keys;
        private bool _open;
        private int _errorsSeen;
        private int _linesVersion = -1;
        private float _smoothedFrameTime;
        private Vector2 _logScroll;
        private GUIStyle _errorStyle;
        private GUIStyle _lineStyle;
        private GUIStyle _panelStyle;
        private Texture2D _panelTexture;

        private void Awake()
        {
            // True in the Editor and in development builds.
            if (!Debug.isDebugBuild)
            {
                enabled = false;
                return;
            }

            _keys = GetComponent<DevKeys>();
            _errorsSeen = LogFile.Recent.ErrorCount;
        }

        private void Update()
        {
            // Unscaled, so the readout stays true under pause and fast speed.
            float dt = Time.unscaledDeltaTime;
            _smoothedFrameTime = _smoothedFrameTime <= 0f ? dt : Mathf.Lerp(_smoothedFrameTime, dt, 0.1f);

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.backquoteKey.wasPressedThisFrame)
            {
                _open = !_open;
                if (_open)
                {
                    _errorsSeen = LogFile.Recent.ErrorCount;
                }
            }
        }

        private void OnGUI()
        {
            // Drawn for a 720-pixel-tall screen and scaled, so it reads the same at any resolution.
            float scale = Mathf.Max(1f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale;
            float height = Screen.height / scale;

            if (!_open)
            {
                DrawErrorNote();
                return;
            }

            EnsureStyles();
            GUILayout.BeginArea(new Rect(8f, 8f, Mathf.Min(460f, width - 16f), height - 16f), _panelStyle);
            GUILayout.Label("Dev console  (` to close)");
            DrawStats();
            DrawRunButtons();
            DrawLevelButtons();
            DrawSaveAndLogButtons();
            DrawLog();
            GUILayout.EndArea();
        }

        private void DrawErrorNote()
        {
            int newErrors = LogFile.Recent.ErrorCount - _errorsSeen;
            if (newErrors > 0)
            {
                GUI.Label(new Rect(8f, 8f, 360f, 24f), $"{newErrors} error(s) logged. Press ` for the dev console.", GUI.skin.box);
            }
        }

        private void DrawStats()
        {
            LevelRun run = LevelRun.Current;
            float fps = _smoothedFrameTime > 0f ? 1f / _smoothedFrameTime : 0f;
            string level = SceneLoader.Current != null ? SceneLoader.Current.OpenLevelId ?? "home screen" : "no SceneLoader";
            GUILayout.Label($"{fps:0} fps ({_smoothedFrameTime * 1000f:0.0} ms)   time scale {Time.timeScale:0.##}   {level}");

            if (run == null)
            {
                return;
            }

            string wave = run.Spawner != null ? $"wave {run.Spawner.CurrentWaveNumber}/{run.Spawner.TotalWaves}" : "no spawner";
            string honeydew = run.Wallet != null ? $"{run.Wallet.Honeydew.Amount} Honeydew" : "no wallet";
            string hill = run.Queen != null ? $"hill {run.Queen.CurrentHealth:0}/{run.Queen.MaxHealth:0}" : "no Queen";
            GUILayout.Label($"{run.Outcome}   {wave}   {EnemyRegistry.All.Count} enemies   {honeydew}   {hill}");
        }

        private void DrawRunButtons()
        {
            LevelRun run = LevelRun.Current;
            GUI.enabled = run != null && run.Outcome == RunOutcome.Playing;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+Honeydew (F6)")) _keys.GiveHoneydew(run);
            if (GUILayout.Button("Next wave (F7)")) run.StartNextWave();
            if (GUILayout.Button("Speed")) run.ToggleSpeed();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Centipede (F5)")) _keys.SendCentipedeAtQueen(run);
            if (GUILayout.Button("Mantis (F8)")) _keys.SendQueenMantis(run);
            if (GUILayout.Button("Hurt Mantis (F9)")) DevKeys.HurtQueenMantisByAThird();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Win level")) run.EndRunForDev(RunOutcome.Won);
            if (GUILayout.Button("Lose level")) run.EndRunForDev(RunOutcome.Lost);
            GUI.enabled = run != null;
            if (GUILayout.Button("Restart")) run.RestartLevel();
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void DrawLevelButtons()
        {
            SceneLoader loader = SceneLoader.Current;
            GUI.enabled = loader != null && !loader.Busy;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Home")) loader.GoHome();
            foreach (LevelInfo level in LevelCatalogue.All)
            {
                if (GUILayout.Button(level.Title)) loader.LoadLevel(level.Id);
            }

            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void DrawSaveAndLogButtons()
        {
            SaveFile save = SaveStore.File;
            int completed = 0;
            foreach (LevelRecord record in save.Data.levels)
            {
                if (record.completed) completed++;
            }

            GUILayout.Label($"Save v{save.Data.version}{(save.ReadOnly ? " (read-only)" : "")}: {completed} level(s) won");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset save file")) save.ResetToNew();
            if (GUILayout.Button("Open logs folder")) Application.OpenURL("file:///" + LogFile.Folder.Replace('\\', '/'));
            GUILayout.EndHorizontal();
        }

        private void DrawLog()
        {
            if (_linesVersion != LogFile.Recent.Version)
            {
                LogFile.Recent.CopyTo(_lines);
                _linesVersion = LogFile.Recent.Version;
                _logScroll.y = float.MaxValue;
            }

            // Fills what the panel has left, up to the chosen number of lines.
            _logScroll = GUILayout.BeginScrollView(_logScroll, GUILayout.MaxHeight(_logLines * 18f));
            foreach (LogLine line in _lines)
            {
                // Errors are red and marked "!", so the colour isn't the only sign.
                GUILayout.Label(line.IsError ? "! " + line.Message : line.Message, line.IsError ? _errorStyle : _lineStyle);
            }

            GUILayout.EndScrollView();
        }

        private void EnsureStyles()
        {
            if (_lineStyle != null)
            {
                return;
            }

            _lineStyle = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 12 };
            _errorStyle = new GUIStyle(_lineStyle);
            _errorStyle.normal.textColor = new Color(1f, 0.45f, 0.4f);

            // Nearly opaque, so the HUD underneath doesn't show through the text.
            _panelTexture = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            _panelTexture.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.1f, 0.94f));
            _panelTexture.Apply();
            _panelStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(8, 8, 6, 6) };
            _panelStyle.normal.background = _panelTexture;
        }

        private void OnDestroy()
        {
            if (_panelTexture != null)
            {
                Destroy(_panelTexture);
            }
        }
    }
}
