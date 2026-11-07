using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// The balance table: every tower and level, every enemy, and each level's waves, side by
    /// side and editable without opening a prefab. Tower rows show attacks per second (1 / interval)
    /// and single-target DPS beside the numbers; enemy rows show shield apart from health; wave rows
    /// are grouped (enemy, count, interval, delay). Edits go through SerializedObjects, so Undo works
    /// and the Inspector's Min and Range limits hold. The checks beside each table are
    /// <see cref="BalanceValidation"/>.
    /// </summary>
    internal sealed class BalanceTableWindow : EditorWindow
    {
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";
        private const string EnemyDataFolder = "Assets/_Game/Features/Enemies/Data/Enemies";
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";

        private const float NameWidth = 110f;
        private const float Narrow = 44f;
        private const float Cell = 58f;
        private const float Wide = 150f;

        private enum Tab
        {
            Towers,
            Enemies,
            Waves,
        }

        private sealed class TowerRow
        {
            public TowerData Data;
            public SerializedObject So;
            public BalanceValidation.TowerShape Shape;
            public bool UsesCone;
            public bool UsesArea;
            public bool UsesSlow;
            public bool UsesMines;
            public bool UsesPoison;
            public string Tempo;
        }

        private sealed class EnemyRow
        {
            public EnemyData Data;
            public SerializedObject So;
            public GameObject Prefab;
            public BalanceValidation.EnemyShape Shape;
        }

        private sealed class LevelSource
        {
            public string ScenePath;
            public string Level;
            public MapWaveDataSO Assigned;
            public int LookupCount;
            public int InlineCount;
        }

        private Tab _tab;
        private Vector2 _scroll;
        private float _previewTempo = 1f;
        private readonly List<TowerRow> _towers = new List<TowerRow>();
        private readonly List<EnemyRow> _enemies = new List<EnemyRow>();
        private readonly List<LevelSource> _levels = new List<LevelSource>();
        private MapWaveDataSO _editingWaves;
        private SerializedObject _wavesSo;
        private readonly List<string> _problems = new List<string>();

        [MenuItem("Tools/Hold the Hill/Balance/Balance Table")]
        private static void Open()
        {
            var window = GetWindow<BalanceTableWindow>("Balance Table");
            window.minSize = new Vector2(900f, 400f);
            window.Show();
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;
            Reload();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
        }

        private void OnProjectChange()
        {
            Reload();
        }

        // ---- Loading --------------------------------------------------------------------------

        private void Reload()
        {
            _towers.Clear();
            foreach (string guid in AssetDatabase.FindAssets("t:TowerData", new[] { TowerDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<TowerData>(AssetDatabase.GUIDToAssetPath(guid));
                if (data != null)
                {
                    _towers.Add(MakeTowerRow(data));
                }
            }
            _towers.Sort((a, b) => KeyIndex(a.Data.name).CompareTo(KeyIndex(b.Data.name)));

            _enemies.Clear();
            foreach (string guid in AssetDatabase.FindAssets("t:EnemyData", new[] { EnemyDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid));
                if (data != null)
                {
                    _enemies.Add(MakeEnemyRow(data));
                }
            }
            _enemies.Sort((a, b) => string.CompareOrdinal(a.Data.name, b.Data.name));

            if (_editingWaves != null)
            {
                _wavesSo = new SerializedObject(_editingWaves);
            }
        }

        private static int KeyIndex(string name)
        {
            int index = System.Array.IndexOf(BalanceDataBuilder.TowerKeyOrder, name);
            return index >= 0 ? index : BalanceDataBuilder.TowerKeyOrder.Length;
        }

        private static TowerRow MakeTowerRow(TowerData data)
        {
            var row = new TowerRow { Data = data, So = new SerializedObject(data) };
            Tower prefab = data.Prefab;
            if (prefab == null)
            {
                row.Tempo = "-";
                return row;
            }

            var projectile = new SerializedObject(prefab).FindProperty("_projectilePrefab").objectReferenceValue as Projectile;
            row.Shape.HasProjectile = projectile != null;
            row.Shape.HasAbility = prefab.GetComponent<ITowerAbility>() != null;
            row.Shape.PrefabPointsBack = prefab.Definition == data;
            row.UsesCone = prefab.GetComponent<FlameJet>() != null;
            for (int i = 0; !row.UsesCone && i < data.LevelCount; i++)
            {
                row.UsesCone = data.Levels[i].Ability.ConeDegrees > 0f;
            }

            var effect = projectile != null ? new SerializedObject(projectile).FindProperty("_hitEffect").objectReferenceValue as GameObject : null;
            bool leavesPuddle = effect != null && effect.GetComponent<ITowerEffect>() != null;
            row.UsesMines = prefab.GetComponent<MineLayer>() != null;
            row.UsesSlow = leavesPuddle;
            row.UsesArea = row.UsesMines || leavesPuddle || (projectile != null && projectile.IsLobbed);
            row.UsesPoison = prefab.GetComponent<PoisonOnHits>() != null;

            // What the Swarm Frenzy tempo (TowerTempo.FireRateFactor) scales for this tower: every use
            // of its attack interval. Durations (the puddle, the poison) are not attack timing.
            row.Tempo = row.UsesMines ? "mines" : row.Shape.HasAbility ? "-" : row.Shape.HasProjectile ? "shots" : "swings";
            return row;
        }

        private static EnemyRow MakeEnemyRow(EnemyData data)
        {
            var row = new EnemyRow { Data = data, So = new SerializedObject(data) };
            row.Prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{EnemyPrefabFolder}/Enemy_{data.name}.prefab");
            if (row.Prefab != null)
            {
                row.Shape.HasShield = row.Prefab.GetComponent<RhinoBeetleShield>() != null;
                row.Shape.HasCurl = row.Prefab.GetComponent<RollyPollyCurler>() != null;
                row.Shape.HasSplit = row.Prefab.GetComponent<CentipedeSplitter>() != null;
            }
            return row;
        }

        // ---- Drawing --------------------------------------------------------------------------

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _tab = (Tab)GUILayout.Toolbar((int)_tab, new[] { "Towers", "Enemies", "Waves" }, EditorStyles.toolbarButton, GUILayout.Width(300f));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                Reload();
            }
            EditorGUILayout.EndHorizontal();

            _problems.Clear();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            switch (_tab)
            {
                case Tab.Towers:
                    DrawTowers();
                    break;
                case Tab.Enemies:
                    DrawEnemies();
                    break;
                default:
                    DrawWaves();
                    break;
            }
            EditorGUILayout.EndScrollView();

            if (_problems.Count > 0)
            {
                EditorGUILayout.HelpBox(string.Join("\n", _problems), MessageType.Warning);
            }
        }

        private static void Header(params (string label, float width)[] columns)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < columns.Length; i++)
            {
                GUILayout.Label(columns[i].label, EditorStyles.miniBoldLabel, GUILayout.Width(columns[i].width));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void Field(SerializedProperty property, float width)
        {
            EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(width));
        }

        private static void FieldOrDash(SerializedProperty property, bool used, float width)
        {
            if (used)
            {
                Field(property, width);
            }
            else
            {
                GUILayout.Label("-", GUILayout.Width(width));
            }
        }

        private static void Computed(string text, float width)
        {
            GUILayout.Label(text, EditorStyles.label, GUILayout.Width(width));
        }

        private static bool SelectButton(Object asset)
        {
            if (!GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(Cell)))
            {
                return false;
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            return true;
        }

        // ---- Towers ---------------------------------------------------------------------------

        private void DrawTowers()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Preview tempo", GUILayout.Width(90f));
            _previewTempo = Mathf.Max(1f, EditorGUILayout.FloatField(_previewTempo, GUILayout.Width(Narrow)));
            GUILayout.Label("(Swarm Frenzy's fire-rate factor; 1 is normal. It divides the attack interval for swings, shots and mine laying alike. Puddle seconds and poison ticks are durations and are not scaled.)",
                EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            Header(("Tower", NameWidth), ("Lvl", 30f), ("Cost", Cell), ("Damage", Cell), ("Interval s", Cell), ("Range", Cell),
                ("APS", Narrow), ("DPS", Cell), ("Cone", Cell), ("Area", Cell), ("Slow", Cell), ("Slow s", Cell), ("Mines", Narrow),
                ("Stacks", Narrow), ("Share", Cell), ("Tempo", Cell), ("", Cell));

            for (int t = 0; t < _towers.Count; t++)
            {
                DrawTower(_towers[t]);
            }
        }

        private void DrawTower(TowerRow row)
        {
            SerializedObject so = row.So;
            so.Update();
            SerializedProperty levels = so.FindProperty("_levels");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(row.Data.towerName, EditorStyles.boldLabel, GUILayout.Width(NameWidth));
            GUILayout.Label("prefab", GUILayout.Width(Narrow));
            SerializedProperty prefabProperty = so.FindProperty("_prefab");
            var pickedPrefab = (Tower)EditorGUILayout.ObjectField(GUIContent.none, prefabProperty.objectReferenceValue, typeof(Tower), false, GUILayout.Width(Wide));
            if (pickedPrefab != prefabProperty.objectReferenceValue)
            {
                prefabProperty.objectReferenceValue = pickedPrefab;
            }
            if (GUILayout.Button("+ level", EditorStyles.miniButton, GUILayout.Width(Cell)))
            {
                levels.arraySize++;
                if (levels.arraySize == 1)
                {
                    BalanceDataBuilder.WriteLevel(levels.GetArrayElementAtIndex(0), TowerLevel.Default);
                }
            }
            if (levels.arraySize > 1 && GUILayout.Button("- level", EditorStyles.miniButton, GUILayout.Width(Cell)))
            {
                levels.arraySize--;
            }
            GUILayout.FlexibleSpace();
            SelectButton(row.Data);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < levels.arraySize; i++)
            {
                SerializedProperty level = levels.GetArrayElementAtIndex(i);
                SerializedProperty ability = level.FindPropertyRelative("Ability");
                float damage = level.FindPropertyRelative("Damage").floatValue;
                float interval = TowerTempo.IntervalUnder(level.FindPropertyRelative("AttackInterval").floatValue, _previewTempo);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(NameWidth));
                GUILayout.Label($"L{i + 1}", GUILayout.Width(30f));
                Field(level.FindPropertyRelative("Cost"), Cell);
                Field(level.FindPropertyRelative("Damage"), Cell);
                Field(level.FindPropertyRelative("AttackInterval"), Cell);
                Field(level.FindPropertyRelative("Range"), Cell);
                Computed(TowerLevel.AttacksPerSecondFor(interval).ToString("F2"), Narrow);
                Computed(TowerLevel.DpsFor(damage, interval).ToString("F1"), Cell);
                FieldOrDash(ability.FindPropertyRelative("ConeDegrees"), row.UsesCone, Cell);
                FieldOrDash(ability.FindPropertyRelative("AreaRadius"), row.UsesArea, Cell);
                FieldOrDash(ability.FindPropertyRelative("SlowMultiplier"), row.UsesSlow, Cell);
                FieldOrDash(ability.FindPropertyRelative("SlowSeconds"), row.UsesSlow, Cell);
                FieldOrDash(ability.FindPropertyRelative("MaxMines"), row.UsesMines, Narrow);
                FieldOrDash(ability.FindPropertyRelative("PoisonMaxStacks"), row.UsesPoison, Narrow);
                FieldOrDash(ability.FindPropertyRelative("PoisonDamageShare"), row.UsesPoison, Cell);
                Computed(i == 0 ? row.Tempo : "", Cell);
                EditorGUILayout.EndHorizontal();
            }

            if (so.ApplyModifiedProperties())
            {
                row.Shape.PrefabPointsBack = row.Data.Prefab != null && row.Data.Prefab.Definition == row.Data;
            }

            _problems.AddRange(BalanceValidation.CheckTower(row.Data, row.Shape));
            EditorGUILayout.Space(4f);
        }

        // ---- Enemies --------------------------------------------------------------------------

        private void DrawEnemies()
        {
            GUILayout.Label("Health and shield are separate pools: the shield soaks hits first and grows back. Columns for an ability the enemy's prefab lacks show a dash.",
                EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.Space();

            Header(("Enemy", NameWidth), ("Health", Cell), ("Shield", Cell), ("Both", Narrow), ("Speed", Cell), ("Damage", Cell), ("Bounty", Cell),
                ("Flying", Narrow), ("Click", Narrow), ("Regrow s", Cell), ("Regrow/s", Cell), ("Curls", Narrow), ("Roll", Cell), ("Share", Cell),
                ("Roll x", Cell), ("Splits", Narrow), ("Half", Cell), ("Fwd", Cell), ("Child x", Cell), ("", Cell));

            for (int e = 0; e < _enemies.Count; e++)
            {
                DrawEnemy(_enemies[e]);
            }
        }

        private void DrawEnemy(EnemyRow row)
        {
            SerializedObject so = row.So;
            so.Update();
            SerializedProperty abilities = so.FindProperty("Abilities");
            float shield = row.Shape.HasShield ? abilities.FindPropertyRelative("ShieldCapacity").floatValue : 0f;
            double health = so.FindProperty("MaxHealth").doubleValue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(row.Data.name, EditorStyles.boldLabel, GUILayout.Width(NameWidth));
            Field(so.FindProperty("MaxHealth"), Cell);
            FieldOrDash(abilities.FindPropertyRelative("ShieldCapacity"), row.Shape.HasShield, Cell);
            Computed((health + shield).ToString("F0"), Narrow);
            Field(so.FindProperty("MoveSpeed"), Cell);
            Field(so.FindProperty("Damage"), Cell);
            Field(so.FindProperty("Honeydew"), Cell);
            Field(so.FindProperty("IsFlying"), Narrow);
            Field(so.FindProperty("DiesToClick"), Narrow);
            FieldOrDash(abilities.FindPropertyRelative("ShieldRegrowDelay"), row.Shape.HasShield, Cell);
            FieldOrDash(abilities.FindPropertyRelative("ShieldRegrowPerSecond"), row.Shape.HasShield, Cell);
            FieldOrDash(abilities.FindPropertyRelative("MaxCurls"), row.Shape.HasCurl, Narrow);
            FieldOrDash(abilities.FindPropertyRelative("RollDistance"), row.Shape.HasCurl, Cell);
            FieldOrDash(abilities.FindPropertyRelative("CurledDamageShare"), row.Shape.HasCurl, Cell);
            FieldOrDash(abilities.FindPropertyRelative("CurledSpeedMultiplier"), row.Shape.HasCurl, Cell);
            FieldOrDash(abilities.FindPropertyRelative("Splits"), row.Shape.HasSplit, Narrow);
            FieldOrDash(abilities.FindPropertyRelative("SplitSizeMultiplier"), row.Shape.HasSplit, Cell);
            FieldOrDash(abilities.FindPropertyRelative("SplitForwardDistance"), row.Shape.HasSplit, Cell);
            FieldOrDash(abilities.FindPropertyRelative("ChildSpeedFactor"), row.Shape.HasSplit, Cell);
            SelectButton(row.Data);
            EditorGUILayout.EndHorizontal();

            so.ApplyModifiedProperties();
            _problems.AddRange(BalanceValidation.CheckEnemy(row.Data, row.Shape));
            if (row.Prefab == null)
            {
                _problems.Add($"{row.Data.name}: no Enemy_{row.Data.name}.prefab beside it, so its abilities are unknown.");
            }
        }

        // ---- Waves ----------------------------------------------------------------------------

        private void DrawWaves()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Each level's spawner takes its waves from one assigned asset (its override). Scan reads the level scenes.", EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("Scan level scenes", GUILayout.Width(130f)))
            {
                ScanLevels();
            }
            EditorGUILayout.EndHorizontal();

            Header(("Level", Wide), ("Assigned wave asset", Wide + Cell), ("", Cell), ("", Cell));
            for (int i = 0; i < _levels.Count; i++)
            {
                LevelSource source = _levels[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(source.Level, EditorStyles.boldLabel, GUILayout.Width(Wide));
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(source.Assigned, typeof(MapWaveDataSO), false, GUILayout.Width(Wide + Cell));
                }
                if (source.Assigned != null && GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(Cell)))
                {
                    SetEditingWaves(source.Assigned);
                }
                if (source.Assigned != null)
                {
                    SelectButton(source.Assigned);
                }
                EditorGUILayout.EndHorizontal();
                _problems.AddRange(BalanceValidation.CheckWaveSource(source.Level, source.Assigned, source.LookupCount, source.InlineCount));
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Wave asset", GUILayout.Width(90f));
            var picked = (MapWaveDataSO)EditorGUILayout.ObjectField(_editingWaves, typeof(MapWaveDataSO), false, GUILayout.Width(Wide + Cell));
            if (picked != _editingWaves)
            {
                SetEditingWaves(picked);
            }
            EditorGUILayout.EndHorizontal();

            if (_editingWaves == null || _wavesSo == null)
            {
                return;
            }

            DrawWaveTable();
        }

        private void SetEditingWaves(MapWaveDataSO waves)
        {
            _editingWaves = waves;
            _wavesSo = waves != null ? new SerializedObject(waves) : null;
        }

        private void DrawWaveTable()
        {
            SerializedObject so = _wavesSo;
            so.Update();
            SerializedProperty waves = so.FindProperty("waves");

            Header(("Wave", Wide), ("Enemy", Wide), ("Count", Narrow), ("Interval s", Cell), ("Delay after s", Cell + 20f), ("", 30f));
            for (int w = 0; w < waves.arraySize; w++)
            {
                SerializedProperty wave = waves.GetArrayElementAtIndex(w);
                SerializedProperty groups = wave.FindPropertyRelative("groups");
                SerializedProperty singles = wave.FindPropertyRelative("enemies");
                Wave live = w < _editingWaves.waves.Count ? _editingWaves.waves[w] : null;

                EditorGUILayout.BeginHorizontal();
                Field(wave.FindPropertyRelative("waveName"), Wide);
                if (GUILayout.Button("+ row", EditorStyles.miniButton, GUILayout.Width(Cell)))
                {
                    groups.arraySize++;
                    SerializedProperty added = groups.GetArrayElementAtIndex(groups.arraySize - 1);
                    if (groups.arraySize == 1)
                    {
                        added.FindPropertyRelative("count").intValue = 1;
                        added.FindPropertyRelative("spawnInterval").floatValue = 1f;
                        added.FindPropertyRelative("delayAfter").floatValue = 1f;
                    }
                }
                if (live != null)
                {
                    GUILayout.Label($"{WaveSpawnPlan.Count(live)} enemies over {WaveSpawnPlan.SpawnSeconds(live):F1} s"
                                    + (singles.arraySize > 0 ? $", {singles.arraySize} single entries (Inspector)" : ""), EditorStyles.miniLabel);
                }
                EditorGUILayout.EndHorizontal();

                for (int g = 0; g < groups.arraySize; g++)
                {
                    SerializedProperty group = groups.GetArrayElementAtIndex(g);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(Wide));
                    Field(group.FindPropertyRelative("enemyPrefab"), Wide);
                    Field(group.FindPropertyRelative("count"), Narrow);
                    Field(group.FindPropertyRelative("spawnInterval"), Cell);
                    Field(group.FindPropertyRelative("delayAfter"), Cell + 20f);
                    if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(30f)))
                    {
                        groups.DeleteArrayElementAtIndex(g);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ wave", EditorStyles.miniButton, GUILayout.Width(Cell)))
            {
                waves.arraySize++;
                SerializedProperty added = waves.GetArrayElementAtIndex(waves.arraySize - 1);
                added.FindPropertyRelative("waveName").stringValue = $"Wave {waves.arraySize}";
                added.FindPropertyRelative("groups").arraySize = 0;
                added.FindPropertyRelative("enemies").arraySize = 0;
            }
            if (waves.arraySize > 0 && GUILayout.Button("- wave", EditorStyles.miniButton, GUILayout.Width(Cell)))
            {
                waves.arraySize--;
            }
            GUILayout.FlexibleSpace();
            SelectButton(_editingWaves);
            EditorGUILayout.EndHorizontal();

            so.ApplyModifiedProperties();
            _problems.AddRange(BalanceValidation.CheckWaves(_editingWaves));
        }

        // Reads each level scene's spawner: which asset is assigned, and whether the other two
        // sources (the lookup list, the inline configurations) hold anything. A scene the user has
        // open is read in place; the rest are opened beside it and closed again.
        private void ScanLevels()
        {
            _levels.Clear();
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                _problems.Add("Stop Play mode to scan the level scenes.");
                return;
            }

            string[] scenes = BalanceDataBuilder.GameplayScenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[i]) == null)
                {
                    continue;
                }

                Scene scene = SceneManager.GetSceneByPath(scenes[i]);
                bool wasOpen = scene.IsValid() && scene.isLoaded;
                if (!wasOpen)
                {
                    scene = EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Additive);
                }

                var source = new LevelSource { ScenePath = scenes[i], Level = System.IO.Path.GetFileNameWithoutExtension(scenes[i]) };
                EnemySpawner spawner = null;
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    spawner = root.GetComponentInChildren<EnemySpawner>(true);
                    if (spawner != null)
                    {
                        break;
                    }
                }

                if (spawner != null)
                {
                    var so = new SerializedObject(spawner);
                    source.Assigned = so.FindProperty("activeMapConfigOverride").objectReferenceValue as MapWaveDataSO;
                    source.LookupCount = so.FindProperty("scriptableMapConfigs").arraySize;
                    source.InlineCount = so.FindProperty("inlineMapConfigs").arraySize;
                }

                if (!wasOpen)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                _levels.Add(source);
            }

            if (_editingWaves == null && _levels.Count > 0)
            {
                SetEditingWaves(_levels[_levels.Count - 1].Assigned);
            }
        }
    }
}
