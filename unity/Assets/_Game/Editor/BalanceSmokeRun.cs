using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HoldTheHill.Core;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// A repeatable combat scenario for checking balance changes against the real game, not a
    /// model (docs/BALANCE_SIM_DECISION.md): opens Bootstrap, enters Play, loads L01_Meadow, builds
    /// all six towers from their definitions beside the road, sends the level's first waves through
    /// Jason's spawner and measures each wave: seconds until the last enemy is gone, enemies sent,
    /// kills, leaks, damage dealt and attacks per tower. After wave 1 it upgrades two towers and logs
    /// the numbers they took from their definitions. Writes PNGs and <c>smoke-log.txt</c> to the
    /// output folder, then a picture of the Balance Table window, then quits.
    /// </summary>
    /// <remarks>
    /// Run windowed (no -batchmode) through the session lock with <c>-balanceSmoke &lt;outputDir&gt;</c>.
    /// The driver is installed when Play mode has been entered (not from the static constructor:
    /// the editor can enter Play without a domain reload, and then the constructor never runs again).
    /// </remarks>
    [InitializeOnLoad]
    internal static class BalanceSmokeRun
    {
        private const string DirKey = "BalanceSmoke.dir";
        private const string DoneKey = "BalanceSmoke.done";
        private const string Level = "L01_Meadow";
        private const int WavesToPlay = 4;
        private const float WaveTimeLimit = 120f;

        private enum Phase { WaitLoader, WaitLevel, StartWave, Wave, Done }

        private sealed class WaveStats
        {
            public int Number;
            public float Start;
            public float End;
            public int Sent;
            public int Kills;
            public int Leaks;
            public double Damage;
            public readonly Dictionary<string, int> Attacks = new Dictionary<string, int>();
        }

        private static string s_dir;
        private static Phase s_step;
        private static float s_stepStart;
        private static bool s_installed;
        private static int s_wave;
        private static WaveStats s_current;
        private static readonly List<WaveStats> s_waves = new List<WaveStats>();
        private static readonly List<Tower> s_towers = new List<Tower>();
        private static readonly StringBuilder s_log = new StringBuilder();
        private static Camera s_camera;
        private static RenderTexture s_rt;
        private static int s_shots;
        private static int s_editorFrames;

        static BalanceSmokeRun()
        {
            string dir = ArgAfter("-balanceSmoke");
            if (dir == null)
            {
                return;
            }

            SessionState.SetString(DirKey, dir);
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            if (Application.isPlaying)
            {
                EditorApplication.delayCall += Install;
            }
            else if (!EditorApplication.isPlayingOrWillChangePlaymode && !SessionState.GetBool(DoneKey, false))
            {
                EditorApplication.delayCall += () =>
                {
                    Directory.CreateDirectory(dir);
                    EditorSceneManager.OpenScene("Assets/_Game/Scenes/Bootstrap.unity");
                    Debug.Log("BalanceSmoke: Bootstrap open, entering Play");
                    EditorApplication.EnterPlaymode();
                };
            }
        }

        private static string ArgAfter(string flag)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == flag)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            Debug.Log("BalanceSmoke: play mode " + change);
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                Install();
            }

            if (change == PlayModeStateChange.EnteredEditMode && SessionState.GetBool(DoneKey, false))
            {
                // Back in edit mode: show the window the designer uses, picture it, then quit.
                s_dir = SessionState.GetString(DirKey, s_dir);
                s_editorFrames = 0;
                EditorApplication.update += CaptureWindowThenQuit;
            }
        }

        private static void Install()
        {
            if (s_installed || !Application.isPlaying)
            {
                return;
            }

            s_installed = true;
            s_dir = SessionState.GetString(DirKey, null) ?? ArgAfter("-balanceSmoke");
            Directory.CreateDirectory(s_dir);
            Time.captureDeltaTime = 1f / 30f;
            Next(Phase.WaitLoader);

            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (loop.subSystemList[i].type == typeof(PostLateUpdate))
                {
                    var subs = new List<PlayerLoopSystem>(loop.subSystemList[i].subSystemList);
                    subs.Insert(0, new PlayerLoopSystem { type = typeof(BalanceSmokeRun), updateDelegate = Tick });
                    loop.subSystemList[i].subSystemList = subs.ToArray();
                }
            }
            PlayerLoop.SetPlayerLoop(loop);

            BasicEnemy.OnAnyDied += enemy => { if (s_current != null) { s_current.Kills++; } };
            BasicEnemy.OnAnyDestinationReached += enemy =>
            {
                if (s_current != null)
                {
                    s_current.Leaks++;
                    Log($"LEAK {enemy.name} reached the Queen");
                }
            };
            BasicEnemy.OnAnyDamaged += (enemy, hit, applied) => { if (s_current != null && applied > 0f) { s_current.Damage += applied; } };
            Log("installed; Unity " + Application.unityVersion);
        }

        private static void Log(string line)
        {
            s_log.AppendLine($"[t={Time.time,7:F2}] {line}");
            Debug.Log("BalanceSmoke: " + line);
        }

        private static void Next(Phase step)
        {
            s_step = step;
            s_stepStart = Time.time;
        }

        private static void Tick()
        {
            try
            {
                Advance();
            }
            catch (Exception ex)
            {
                Log("EXCEPTION " + ex);
                Finish(1);
            }
        }

        private static void Advance()
        {
            float since = Time.time - s_stepStart;
            LevelRun run = LevelRun.Current;
            switch (s_step)
            {
                case Phase.WaitLoader:
                    if (SceneLoader.Current != null && !SceneLoader.Current.Busy)
                    {
                        Log($"loading {Level}");
                        SceneLoader.Current.LoadLevel(Level);
                        Next(Phase.WaitLevel);
                    }
                    else if (since > 20f) { Log("no SceneLoader"); Finish(2); }
                    break;

                case Phase.WaitLevel:
                    if (run != null && SceneLoader.Current.OpenLevelId == Level && !SceneLoader.Current.Busy
                        && Object.FindAnyObjectByType<EnemyPath>() != null && since > 1f)
                    {
                        SetUpLevel(run);
                        Shot("1-towers-built");
                        s_wave = 0;
                        Next(Phase.StartWave);
                    }
                    else if (since > 30f) { Log("level did not open"); Finish(3); }
                    break;

                case Phase.StartWave:
                    if (run.Spawner.CurrentState == EnemySpawner.SpawnerState.Completed || s_wave >= WavesToPlay)
                    {
                        Shot("4-end");
                        Summarise(run);
                        Finish(0);
                        break;
                    }

                    if (since < 0.5f)
                    {
                        break;
                    }

                    s_wave++;
                    s_current = new WaveStats { Number = s_wave, Start = Time.time };
                    s_waves.Add(s_current);
                    run.StartNextWave();
                    Log($"WAVE {s_wave} started; spawner {run.Spawner.CurrentState}; {WaveSpawnPlan.Count(WaveAt(run, s_wave - 1))} enemies planned");
                    if (run.Spawner.CurrentState != EnemySpawner.SpawnerState.Spawning)
                    {
                        Log("the spawner did not start the wave (CanSendNextWave was false)");
                        Finish(4);
                        break;
                    }
                    Next(Phase.Wave);
                    break;

                case Phase.Wave:
                    bool spawning = run.Spawner.CurrentState == EnemySpawner.SpawnerState.Spawning;
                    if (s_wave == 2 && since > 6f && s_shots < 3)
                    {
                        Shot("3-wave2-fighting");
                    }

                    if (!spawning && EnemyRegistry.All.Count == 0 && since > 1f)
                    {
                        EndWave(run);
                        if (s_wave == 1)
                        {
                            Shot("2-after-wave1");
                            UpgradeTwoTowers();
                        }
                        Next(Phase.StartWave);
                    }
                    else if (since > WaveTimeLimit)
                    {
                        Log($"wave {s_wave} ran past {WaveTimeLimit} s with {EnemyRegistry.All.Count} enemies left; stopping");
                        EndWave(run);
                        Shot("4-timeout");
                        Summarise(run);
                        Finish(5);
                    }
                    break;
            }
        }

        private static Wave WaveAt(LevelRun run, int index)
        {
            var so = new SerializedObject(run.Spawner);
            var asset = so.FindProperty("activeMapConfigOverride").objectReferenceValue as MapWaveDataSO;
            return asset != null && index < asset.waves.Count ? asset.waves[index] : null;
        }

        // All six towers from their definitions, beside successive stretches of the road, alternating sides.
        private static void SetUpLevel(LevelRun run)
        {
            EnemyPath path = Object.FindAnyObjectByType<EnemyPath>();
            IReadOnlyList<Vector3> road = path.Waypoints;
            Log($"level open; road has {road.Count} waypoints, {RoadLength(road):F1} units long; spawner on {new SerializedObject(run.Spawner).FindProperty("activeMapConfigOverride").objectReferenceValue?.name}");

            IReadOnlyList<TowerData> offers = run.Placer != null ? run.Placer.Offers : null;
            if (offers == null || offers.Count == 0)
            {
                Log("the placer offers nothing");
                return;
            }

            for (int i = 0; i < offers.Count; i++)
            {
                TowerData data = offers[i];
                if (data == null || data.Prefab == null)
                {
                    Log($"offer {i + 1} has no prefab");
                    continue;
                }

                int w = Mathf.Min(1 + i, road.Count - 1);
                Vector3 a = road[w - 1], b = road[w];
                Vector3 mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 side = Vector3.Cross((b - a).normalized, Vector3.forward) * (i % 2 == 0 ? 1.4f : -1.4f);
                Tower tower = Object.Instantiate(data.Prefab, mid + side, Quaternion.identity, run.Placer.transform);
                tower.name = data.name;
                tower.ShopData = data;
                tower.HoneydewSpent = data.honeydewCost;
                s_towers.Add(tower);

                string name = data.name;
                tower.Attacked += () => Count(name);
                tower.AbilityUsed += () => Count(name);
                Log($"TOWER {name} L{tower.Level}/{tower.MaxLevel} at {tower.transform.position:F1}: range {tower.Range}, damage {tower.Damage}, every {tower.FireInterval} s "
                    + $"(dps {tower.Current.SingleTargetDps:F1}), ability {Describe(tower.Ability)}, definition {(tower.Definition != null ? tower.Definition.name : "NONE")}");
            }

            run.Spawner.onEnemySpawned.AddListener(_ => { if (s_current != null) { s_current.Sent++; } });

            Camera main = Camera.main;
            s_rt = new RenderTexture(1920, 1080, 24);
            var go = new GameObject("SmokeCamera");
            s_camera = go.AddComponent<Camera>();
            s_camera.CopyFrom(main);
            s_camera.rect = new Rect(0, 0, 1, 1);
            s_camera.ResetProjectionMatrix();
            s_camera.targetTexture = s_rt;
            s_camera.depth = main.depth + 1;
            go.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
        }

        private static void Count(string tower)
        {
            if (s_current == null)
            {
                return;
            }

            s_current.Attacks.TryGetValue(tower, out int n);
            s_current.Attacks[tower] = n + 1;
        }

        private static string Describe(TowerAbilitySettings a)
        {
            var parts = new List<string>();
            if (a.ConeDegrees > 0f) parts.Add($"cone {a.ConeDegrees}");
            if (a.AreaRadius > 0f) parts.Add($"area {a.AreaRadius}");
            if (a.SlowMultiplier > 0f) parts.Add($"slow x{a.SlowMultiplier} for {a.SlowSeconds} s");
            if (a.MaxMines > 0) parts.Add($"{a.MaxMines} mines");
            if (a.PoisonMaxStacks > 0) parts.Add($"poison {a.PoisonMaxStacks} stacks at {a.PoisonDamageShare}");
            return parts.Count > 0 ? string.Join(", ", parts) : "none";
        }

        private static float RoadLength(IReadOnlyList<Vector3> road)
        {
            float total = 0f;
            for (int i = 1; i < road.Count; i++) total += Vector3.Distance(road[i - 1], road[i]);
            return total;
        }

        // The data path for an upgrade: the Bullet Ant and the Carpenter Ant take their level-2 rows.
        private static void UpgradeTwoTowers()
        {
            foreach (Tower tower in s_towers)
            {
                if (tower == null || (tower.name != "BulletAnt" && tower.name != "CarpenterAnt"))
                {
                    continue;
                }

                string before = $"L{tower.Level} {tower.Range}/{tower.Damage}/{tower.FireInterval} s, {Describe(tower.Ability)}";
                int cost = tower.NextUpgradeCost;
                bool upgraded = tower.Upgrade();
                MineLayer layer = tower.GetComponent<MineLayer>();
                Log($"UPGRADE {tower.name}: {before} -> paid {cost}, now L{tower.Level} {tower.Range}/{tower.Damage}/{tower.FireInterval} s, {Describe(tower.Ability)}"
                    + (layer != null ? $", mine cap {layer.MaxMines}" : "") + (upgraded ? "" : " (REFUSED)"));
            }
        }

        private static void EndWave(LevelRun run)
        {
            s_current.End = Time.time;
            var attacks = new List<string>();
            foreach (KeyValuePair<string, int> pair in s_current.Attacks)
            {
                attacks.Add($"{pair.Key} {pair.Value}");
            }
            Log($"WAVE {s_current.Number} over in {s_current.End - s_current.Start:F1} s: sent {s_current.Sent}, kills {s_current.Kills}, leaks {s_current.Leaks}, "
                + $"damage dealt {s_current.Damage:F0}, attacks [{string.Join(", ", attacks)}], queen {run.Queen?.CurrentHealth}/{run.Queen?.MaxHealth}, honeydew {run.Wallet?.Honeydew.Amount}");
            s_current = null;
        }

        private static void Summarise(LevelRun run)
        {
            var text = new StringBuilder("SUMMARY wave | seconds | sent | kills | leaks | damage\n");
            foreach (WaveStats wave in s_waves)
            {
                text.AppendLine($"  {wave.Number} | {wave.End - wave.Start,6:F1} | {wave.Sent,4} | {wave.Kills,5} | {wave.Leaks,5} | {wave.Damage,7:F0}");
            }
            text.Append($"  outcome {run.Outcome}, spawner {run.Spawner.CurrentState}, registry {EnemyRegistry.All.Count}");
            Log(text.ToString());
        }

        private static void Shot(string name)
        {
            s_shots++;
            if (s_camera == null) return;
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = s_rt;
            var tex = new Texture2D(s_rt.width, s_rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, s_rt.width, s_rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = previous;
            File.WriteAllBytes(Path.Combine(s_dir, name + ".png"), tex.EncodeToPNG());
            Object.Destroy(tex);
            Log($"SHOT {name}.png");
        }

        private static void Finish(int code)
        {
            if (s_step == Phase.Done) return;
            s_step = Phase.Done;
            Log($"finished with code {code}");
            File.WriteAllText(Path.Combine(s_dir, "smoke-log.txt"), s_log.ToString());
            SessionState.SetBool(DoneKey, true);
            SessionState.SetInt("BalanceSmoke.code", code);
            EditorApplication.ExitPlaymode();
        }

        // Opens the Balance Table, lets it lay out for a few frames, reads it off the screen, quits.
        private static void CaptureWindowThenQuit()
        {
            s_editorFrames++;
            if (s_editorFrames == 1)
            {
                var window = EditorWindow.GetWindow<BalanceTableWindow>("Balance Table");
                window.position = new Rect(40f, 60f, 1560f, 720f);
                window.Show();
                window.Focus();
                return;
            }

            if (s_editorFrames < 90)
            {
                EditorWindow.GetWindow<BalanceTableWindow>("Balance Table").Repaint();
                return;
            }

            EditorApplication.update -= CaptureWindowThenQuit;
            try
            {
                var window = EditorWindow.GetWindow<BalanceTableWindow>("Balance Table");
                Rect r = window.position;
                int width = (int)r.width;
                int height = (int)r.height;
                Color[] pixels = InternalEditorUtility.ReadScreenPixel(new Vector2(r.x, r.y), width, height);
                var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.SetPixels(pixels);
                tex.Apply();
                File.WriteAllBytes(Path.Combine(s_dir, "5-balance-table-window.png"), tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                Debug.Log("BalanceSmoke: window captured");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("BalanceSmoke: window capture failed: " + ex.Message);
            }

            EditorApplication.Exit(SessionState.GetInt("BalanceSmoke.code", 0));
        }
    }
}
