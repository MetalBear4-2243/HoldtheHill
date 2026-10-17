using System.Collections.Generic;
using System.Text;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 6, the bosses. Makes the Centipede prefab (Jason's <see cref="CentipedeEnemy"/>
    /// with a <see cref="CentipedeSpeedUp"/> beside it) and the Queen Mantis prefab
    /// (<see cref="QueenMantisBoss"/> raising Mantis Spawn husks), both with a boss-sized
    /// <see cref="Bounty"/>, and puts one of each at the end of the last wave of its level:
    /// the Centipede closes Tutorial Hill, the Queen Mantis closes Meadow.
    /// </summary>
    /// <remarks>
    /// Safe to run again: a prefab that exists is kept (Inspector tuning survives), and a boss is
    /// appended to a last wave only while that wave has none. Runs after Build Enemies, which makes
    /// the Mantis Spawn prefab the Queen raises. No scene is opened.
    /// </remarks>
    internal static class AlphaBossesBuilder
    {
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string WaveDataFolder = "Assets/_Game/Features/Enemies/Data";
        private const string CentipedePath = EnemyPrefabFolder + "/Enemy_Centipede.prefab";
        private const string QueenMantisPath = EnemyPrefabFolder + "/Enemy_QueenMantis.prefab";
        private const string MantisSpawnPath = EnemyPrefabFolder + "/Enemy_MantisSpawn.prefab";

        /// <summary>Seconds between the last ordinary enemy of a boss wave and the boss itself.</summary>
        private const float GapBeforeBoss = 3f;

        private static readonly (string id, string waves, string boss)[] Levels =
        {
            ("L00_TutorialHill", $"{WaveDataFolder}/L00_TutorialHill_Waves.asset", CentipedePath),
            ("L01_Meadow", $"{WaveDataFolder}/L01_Meadow_Waves.asset", QueenMantisPath),
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves)")]
        private static void Build()
        {
            Sprite circle = AlphaCoreLoopBuilder.EnsureSprite("Circle", circle: true);
            AlphaCoreLoopBuilder.EnsureFolder(EnemyPrefabFolder);

            var husk = AssetDatabase.LoadAssetAtPath<GameObject>(MantisSpawnPath);
            if (husk == null)
            {
                Debug.LogWarning($"{MantisSpawnPath} does not exist yet; run Build Enemies first. The Queen Mantis will raise nothing until it is set.");
            }

            EnsureCentipedePrefab(circle);
            EnsureQueenMantisPrefab(circle, husk);
            AssetDatabase.SaveAssets();

            for (int i = 0; i < Levels.Length; i++)
            {
                AppendBossToLastWave(Levels[i].waves, AssetDatabase.LoadAssetAtPath<GameObject>(Levels[i].boss));
            }
            AssetDatabase.SaveAssets();

            ReportAfterReload();
            Debug.Log("Alpha bosses built: a Centipede ends Tutorial Hill, a Queen Mantis ends Meadow.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaBossesBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // Jason's CentipedeEnemy with no split prefab, so each child is a copy of its parent and
        // brings the speed-up, the registry entry and the bounty with it.
        private static GameObject EnsureCentipedePrefab(Sprite circle)
        {
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(CentipedePath);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = NewBossObject("Enemy_Centipede", circle, new Color(0.6f, 0.15f, 0.1f), 0.9f);
            var centipede = go.AddComponent<CentipedeEnemy>();
            var so = new SerializedObject(centipede);
            so.FindProperty("_maxHealth").doubleValue = 60.0;
            so.FindProperty("_damage").intValue = 20;
            so.FindProperty("_moveSpeed").floatValue = 1.1f;
            so.FindProperty("_splitsRemaining").intValue = 2;
            so.FindProperty("_sizeMultiplier").floatValue = 0.8f;
            so.FindProperty("_splitForwardDistance").floatValue = 0.6f;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<RegisteredEnemy>();
            SetBounty(go.AddComponent<Bounty>(), 10);

            var speedUp = go.AddComponent<CentipedeSpeedUp>();
            var speedSo = new SerializedObject(speedUp);
            speedSo.FindProperty("_childSpeedFactor").floatValue = 1.25f;
            speedSo.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, CentipedePath);
            Object.DestroyImmediate(go);
            Debug.Log("Made Enemy_Centipede (CentipedeEnemy + CentipedeSpeedUp; two splits, seven parts in all).");
            return prefab;
        }

        private static GameObject EnsureQueenMantisPrefab(Sprite circle, GameObject husk)
        {
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(QueenMantisPath);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = NewBossObject("Enemy_QueenMantis", circle, new Color(0.25f, 0.6f, 0.2f), 1.1f);
            var queen = go.AddComponent<QueenMantisBoss>();
            var so = new SerializedObject(queen);
            so.FindProperty("_maxHealth").doubleValue = 300.0;
            so.FindProperty("_damage").intValue = 40;
            so.FindProperty("_moveSpeed").floatValue = 0.8f;
            so.FindProperty("_huskPrefab").objectReferenceValue = husk != null ? husk.GetComponent<BasicEnemy>() : null;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<RegisteredEnemy>();
            SetBounty(go.AddComponent<Bounty>(), 80);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, QueenMantisPath);
            Object.DestroyImmediate(go);
            Debug.Log("Made Enemy_QueenMantis (QueenMantisBoss; heals in white, raises Mantis Spawn husks in black).");
            return prefab;
        }

        private static GameObject NewBossObject(string name, Sprite sprite, Color color, float scale)
        {
            var go = new GameObject(name);
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            // Bosses are drawn over the ordinary walkers, under the flyers.
            renderer.sortingOrder = 6;
            return go;
        }

        private static void SetBounty(Bounty bounty, int honeydew)
        {
            var so = new SerializedObject(bounty);
            so.FindProperty("_honeydew").intValue = honeydew;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Puts one boss at the end of the level's last wave, a beat after the last ordinary enemy,
        /// unless that wave already has one.
        /// </summary>
        private static void AppendBossToLastWave(string wavesPath, GameObject boss)
        {
            var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(wavesPath);
            if (waves == null || boss == null)
            {
                Debug.LogWarning($"{wavesPath} or its boss prefab is missing; run Build Home and Levels first.");
                return;
            }

            if (waves.waves.Count == 0)
            {
                Debug.LogWarning($"{waves.name} has no waves; nothing to end with a boss.");
                return;
            }

            Wave last = waves.waves[waves.waves.Count - 1];
            for (int i = 0; i < last.enemies.Count; i++)
            {
                if (last.enemies[i].enemyPrefab == boss)
                {
                    Debug.Log($"{waves.name}: {last.waveName} already ends with {boss.name}; kept as it is.");
                    return;
                }
            }

            if (last.enemies.Count > 0)
            {
                EnemySpawnEntry before = last.enemies[last.enemies.Count - 1];
                before.delayBeforeNext = Mathf.Max(before.delayBeforeNext, GapBeforeBoss);
            }
            last.enemies.Add(new EnemySpawnEntry { enemyPrefab = boss, delayBeforeNext = 0f });

            EditorUtility.SetDirty(waves);
            Debug.Log($"{waves.name}: {boss.name} added to the end of {last.waveName}.");
        }

        // Loads everything again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            var centipede = AssetDatabase.LoadAssetAtPath<GameObject>(CentipedePath);
            if (centipede == null)
            {
                Debug.LogError("Enemy_Centipede is missing after the build.");
            }
            else
            {
                var enemy = centipede.GetComponent<CentipedeEnemy>();
                var speedUp = centipede.GetComponent<CentipedeSpeedUp>();
                Debug.Log($"Enemy_Centipede: {Parts(centipede)}"
                    + (speedUp != null ? $", children x{speedUp.ChildSpeedFactor} speed" : ", NO speed-up")
                    + (enemy != null ? $"; health {enemy.MaxHealth}, speed {enemy.MoveSpeed}, splits {enemy.SplitsRemaining}, split prefab {(enemy.SplitPrefab != null ? enemy.SplitPrefab.name : "none (copies of the parent)")}" : "; NO CentipedeEnemy"));
            }

            var queen = AssetDatabase.LoadAssetAtPath<GameObject>(QueenMantisPath);
            if (queen == null)
            {
                Debug.LogError("Enemy_QueenMantis is missing after the build.");
            }
            else
            {
                var enemy = queen.GetComponent<QueenMantisBoss>();
                string husk = enemy != null ? AlphaRunFrameBuilder.Name(new SerializedObject(enemy), "_huskPrefab") : "NO QueenMantisBoss";
                Debug.Log($"Enemy_QueenMantis: {Parts(queen)}; husk prefab {husk}"
                    + (enemy != null ? $"; health {enemy.MaxHealth}, speed {enemy.MoveSpeed}" : ""));
            }

            for (int i = 0; i < Levels.Length; i++)
            {
                var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(Levels[i].waves);
                if (waves == null || waves.waves.Count == 0)
                {
                    continue;
                }

                Wave last = waves.waves[waves.waves.Count - 1];
                Debug.Log($"{waves.name}: {last.waveName} = {AlphaEnemiesBuilder.Describe(last)}");
            }
        }

        private static string Parts(GameObject prefab)
        {
            var text = new StringBuilder(prefab.GetComponent<BasicEnemy>() != null ? prefab.GetComponent<BasicEnemy>().GetType().Name : "NO BasicEnemy");
            text.Append(prefab.GetComponent<RegisteredEnemy>() != null ? ", registered" : ", NOT registered");
            text.Append(prefab.GetComponent<Bounty>() != null ? $", bounty {prefab.GetComponent<Bounty>().Amount}" : ", NO bounty");
            return text.ToString();
        }
    }
}
