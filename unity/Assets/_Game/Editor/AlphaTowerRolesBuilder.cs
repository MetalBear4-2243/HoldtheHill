using System.Text;
using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Tower roles (2026-11-05, Nilly's design): the Bullet Ant fires a fast bullet; the Sap Ant lobs
    /// a slow honey glob that leaves a sticky puddle (<see cref="SapPuddle"/>) in place of its slow
    /// aura; the Fire Ant is a flamethrower, a short melee cone (<see cref="Tower.ConeDegrees"/>) shown
    /// by a stream of <see cref="FlamePuff"/>s (<see cref="FlameJet"/>); the Crazy Ant hits weaker than
    /// the Ant and poisons on two hits in a row (<see cref="PoisonOnHits"/>). Then Build Sprites dresses
    /// it all (<see cref="AlphaSpritesBuilder"/>).
    /// </summary>
    /// <remarks>
    /// Safe to run again: each tower is changed once, while it still has its old role (the Sap Ant's
    /// aura, the Fire Ant's lob, the Crazy Ant without poison, the Bullet Ant's slow shot); after that
    /// its numbers are the Inspector's. New prefabs are made only when missing. To undo: revert the
    /// tower prefabs and Projectile_Bullet, and delete Projectile_Sap, Fx_SapPuddle and Fx_FlamePuff.
    /// </remarks>
    internal static class AlphaTowerRolesBuilder
    {
        private const string Folder = "Assets/_Game/Features/Towers/Prefabs";

        // Bullet Ant: about two and a half times as fast; a miss is off the map within a second.
        private const float BulletSpeed = 24f;
        private const float BulletLifetime = 1f;

        // Sap Ant: a slow lob (the Fire Ant's was 6) with a low arc, every 2 s, no damage of its own.
        private const float SapSpeed = 3.5f;
        private const float SapArcHeight = 1f;
        private const float SapLifetime = 3f;

        // The puddle: the honey splat's last frame is 20 px wide; at 1.1 it is drawn about 0.7 units
        // across, the puddle's reach (SapPuddle radius 0.35). Under the enemies (5), over the road (0).
        private const float PuddleScale = 1.1f;
        private const int PuddleSortingOrder = 2;

        // Flame puffs: the towers' pixel size, just under the shots (20).
        private const float PuffScale = 0.8f;
        private const int PuffSortingOrder = 19;

        // Fire Ant: a 70-degree cone, 2 units, five times a second.
        private const float FlameConeDegrees = 70f;

        // Level 1, then the two upgrade levels: range, damage, interval. Upgrade prices stay as they are.
        private static readonly Vector3[] SapLevels = { new Vector3(2f, 0f, 2f), new Vector3(2.4f, 0f, 1.8f), new Vector3(2.8f, 0f, 1.6f) };
        private static readonly Vector3[] FireLevels = { new Vector3(2f, 3f, 0.2f), new Vector3(2.2f, 4.5f, 0.2f), new Vector3(2.4f, 6f, 0.2f) };
        private static readonly float[] CrazyDamage = { 4f, 6f, 9f };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Tower Roles (fast bullet, sap shot, flamethrower, poison)")]
        private static void Build()
        {
            BuildAll();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaTowerRolesBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildAll()
        {
            Material material = SpriteMaterial();
            EnsurePrefab("Fx_SapPuddle", material, PuddleScale, PuddleSortingOrder, go => go.AddComponent<SapPuddle>());
            EnsurePrefab("Fx_FlamePuff", material, PuffScale, PuffSortingOrder, go =>
            {
                go.AddComponent<SpriteFlipbook>();
                go.AddComponent<FlamePuff>();
            });
            EnsureSapShot(material);

            SpeedUpBullet();
            MakeSapAntLob();
            MakeFireAntFlame();
            PoisonCrazyAnt();
            AssetDatabase.SaveAssets();

            // The pictures for all of it, and every other sheet as before.
            AlphaSpritesBuilder.BuildFromCommandLine();
            Report();
            Debug.Log("Tower roles built: fast bullet, sap shot and puddles, flamethrower, poison on two hits.");
        }

        private static string PathOf(string prefab) => $"{Folder}/{prefab}.prefab";

        // The shots' own material (the URP sprite material the placeholders were made with).
        private static Material SpriteMaterial()
        {
            var lob = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf("Projectile_Lob"));
            var renderer = lob != null ? lob.GetComponent<SpriteRenderer>() : null;
            return renderer != null ? renderer.sharedMaterial : null;
        }

        private static void EnsurePrefab(string name, Material material, float scale, int sortingOrder, System.Action<GameObject> addParts)
        {
            string path = PathOf(name);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                return;
            }

            var go = new GameObject(name);
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
            renderer.sortingOrder = sortingOrder;
            addParts(go);
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"{path} made.");
        }

        // The Sap Ant's glob: a lob like the old Fire Ant's, slower and lower; it leaves the puddle.
        private static void EnsureSapShot(Material material)
        {
            string path = PathOf("Projectile_Sap");
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                var go = new GameObject("Projectile_Sap");
                var renderer = go.AddComponent<SpriteRenderer>();
                if (material != null)
                {
                    renderer.sharedMaterial = material;
                }
                renderer.sortingOrder = 20;
                var shot = new SerializedObject(go.AddComponent<Projectile>());
                shot.FindProperty("_lobbed").boolValue = true;
                shot.FindProperty("_speed").floatValue = SapSpeed;
                shot.FindProperty("_arcHeight").floatValue = SapArcHeight;
                shot.FindProperty("_splashRadius").floatValue = 0.35f;
                shot.FindProperty("_lifetime").floatValue = SapLifetime;
                shot.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(go, path);
                Object.DestroyImmediate(go);
                Debug.Log($"{path} made.");
            }

            Edit("Projectile_Sap", root =>
            {
                var puddle = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf("Fx_SapPuddle"));
                var so = new SerializedObject(root.GetComponent<Projectile>());
                SerializedProperty effect = so.FindProperty("_hitEffect");
                if (effect.objectReferenceValue == puddle)
                {
                    return false;
                }
                effect.objectReferenceValue = puddle;
                so.ApplyModifiedPropertiesWithoutUndo();
                return true;
            });
        }

        private static void SpeedUpBullet()
        {
            Edit("Projectile_Bullet", root =>
            {
                var so = new SerializedObject(root.GetComponent<Projectile>());
                if (so.FindProperty("_speed").floatValue >= BulletSpeed * 0.75f)
                {
                    return false;
                }
                so.FindProperty("_speed").floatValue = BulletSpeed;
                so.FindProperty("_lifetime").floatValue = BulletLifetime;
                so.ApplyModifiedPropertiesWithoutUndo();
                return true;
            });
        }

        private static void MakeSapAntLob()
        {
            Edit("Tower_SapAnt", root =>
            {
                var aura = root.GetComponent<SlowAura>();
                var tower = new SerializedObject(root.GetComponent<Tower>());
                SerializedProperty shot = tower.FindProperty("_projectilePrefab");
                if (aura == null && shot.objectReferenceValue != null)
                {
                    return false;
                }

                if (aura != null)
                {
                    Object.DestroyImmediate(aura);
                }

                // Loaded after the prefab was opened, as the sprites builder does.
                var sap = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf("Projectile_Sap"));
                shot.objectReferenceValue = sap != null ? sap.GetComponent<Projectile>() : null;
                SetLevels(tower, SapLevels);
                tower.ApplyModifiedPropertiesWithoutUndo();
                return true;
            });
        }

        private static void MakeFireAntFlame()
        {
            Edit("Tower_FireAnt", root =>
            {
                var tower = new SerializedObject(root.GetComponent<Tower>());
                bool changed = false;
                if (tower.FindProperty("_projectilePrefab").objectReferenceValue != null)
                {
                    tower.FindProperty("_projectilePrefab").objectReferenceValue = null;
                    tower.FindProperty("_coneDegrees").floatValue = FlameConeDegrees;
                    SetLevels(tower, FireLevels);
                    tower.ApplyModifiedPropertiesWithoutUndo();
                    changed = true;
                }

                FlameJet jet = root.GetComponent<FlameJet>();
                if (jet == null)
                {
                    jet = root.AddComponent<FlameJet>();
                    changed = true;
                }

                var puff = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf("Fx_FlamePuff"));
                var jetSo = new SerializedObject(jet);
                SerializedProperty puffField = jetSo.FindProperty("_puffPrefab");
                FlamePuff puffComponent = puff != null ? puff.GetComponent<FlamePuff>() : null;
                if (puffField.objectReferenceValue != puffComponent)
                {
                    puffField.objectReferenceValue = puffComponent;
                    jetSo.ApplyModifiedPropertiesWithoutUndo();
                    changed = true;
                }
                return changed;
            });
        }

        private static void PoisonCrazyAnt()
        {
            Edit("Tower_CrazyAnt", root =>
            {
                if (root.GetComponent<PoisonOnHits>() != null)
                {
                    return false;
                }

                root.AddComponent<PoisonOnHits>();
                var tower = new SerializedObject(root.GetComponent<Tower>());
                tower.FindProperty("_damage").floatValue = CrazyDamage[0];
                SerializedProperty upgrades = tower.FindProperty("_upgrades");
                for (int i = 0; i < upgrades.arraySize && i + 1 < CrazyDamage.Length; i++)
                {
                    upgrades.GetArrayElementAtIndex(i).FindPropertyRelative("Damage").floatValue = CrazyDamage[i + 1];
                }
                tower.ApplyModifiedPropertiesWithoutUndo();
                return true;
            });
        }

        // Level 1 on the tower, levels 2 and 3 on its upgrades; each upgrade keeps its price.
        private static void SetLevels(SerializedObject tower, Vector3[] levels)
        {
            tower.FindProperty("_range").floatValue = levels[0].x;
            tower.FindProperty("_damage").floatValue = levels[0].y;
            tower.FindProperty("_fireInterval").floatValue = levels[0].z;
            SerializedProperty upgrades = tower.FindProperty("_upgrades");
            for (int i = 0; i < upgrades.arraySize && i + 1 < levels.Length; i++)
            {
                SerializedProperty level = upgrades.GetArrayElementAtIndex(i);
                level.FindPropertyRelative("Range").floatValue = levels[i + 1].x;
                level.FindPropertyRelative("Damage").floatValue = levels[i + 1].y;
                level.FindPropertyRelative("FireInterval").floatValue = levels[i + 1].z;
            }
        }

        // Opens a prefab, lets the change run, and saves only if it changed something.
        private static void Edit(string prefab, System.Func<GameObject, bool> change)
        {
            string path = PathOf(prefab);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                Debug.LogError($"{path} is missing; run the Alpha core loop and towers builders first.");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (change(root))
                {
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    Debug.Log($"{path} changed.");
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        // ---- Report ---------------------------------------------------------------------------

        private static void Report()
        {
            foreach (string name in new[] { "Tower_BulletAnt", "Tower_SapAnt", "Tower_FireAnt", "Tower_CrazyAnt", "Tower_Ant" })
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf(name));
                Tower tower = prefab != null ? prefab.GetComponent<Tower>() : null;
                if (tower == null)
                {
                    Debug.LogError($"{name} is missing after the build.");
                    continue;
                }

                var so = new SerializedObject(tower);
                var text = new StringBuilder($"{name}: ");
                text.Append($"range {tower.Range}, damage {tower.Damage}, every {tower.FireInterval} s");
                SerializedProperty upgrades = so.FindProperty("_upgrades");
                for (int i = 0; i < upgrades.arraySize; i++)
                {
                    SerializedProperty level = upgrades.GetArrayElementAtIndex(i);
                    text.Append($"; L{i + 2} {level.FindPropertyRelative("Range").floatValue}/{level.FindPropertyRelative("Damage").floatValue}/{level.FindPropertyRelative("FireInterval").floatValue}");
                }
                Object shot = so.FindProperty("_projectilePrefab").objectReferenceValue;
                text.Append(shot != null ? $"; shoots {shot.name}" : "; melee");
                text.Append(tower.ConeDegrees > 0f ? $" in a {tower.ConeDegrees} degree cone" : "");
                text.Append(prefab.GetComponent<SlowAura>() != null ? "; SLOW AURA STILL THERE" : "");
                text.Append(prefab.GetComponent<FlameJet>() != null ? "; flame jet" : "");
                text.Append(prefab.GetComponent<PoisonOnHits>() != null ? "; poisons on hits" : "");
                Debug.Log(text.ToString());
            }

            foreach (string name in new[] { "Projectile_Bullet", "Projectile_Sap" })
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PathOf(name));
                Projectile projectile = prefab != null ? prefab.GetComponent<Projectile>() : null;
                if (projectile == null)
                {
                    Debug.LogError($"{name} is missing after the build.");
                    continue;
                }

                var so = new SerializedObject(projectile);
                Object effect = so.FindProperty("_hitEffect").objectReferenceValue;
                Debug.Log($"{name}: speed {so.FindProperty("_speed").floatValue}, lobbed {projectile.IsLobbed}, leaves {(effect != null ? effect.name : "nothing")}");
            }
        }
    }
}
