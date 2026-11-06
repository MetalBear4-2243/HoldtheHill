using System.IO;
using System.Text;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// The Queen Mantis rebuild (2026-11-02): her art's import settings, a ring sprite, her
    /// <see cref="QueenMantisData"/>, the Enemy_QueenMantis prefab (<see cref="QueenMantisBoss"/> with
    /// the heal aura, raiser, summoner, look and body markers beside it), her place in Meadow's last
    /// wave, the <see cref="QueenMantisBossBar"/> in the UI scene, and the prefab the F8 dev key sends.
    /// Leaves the Bootstrap scene open, ready for Play.
    /// </summary>
    /// <remarks>
    /// Safe to run again. The data asset is made only when missing; after that only its empty
    /// references are filled, so Inspector tuning survives. The prefab is upgraded in place: a
    /// missing piece is added, and a reference or number already set is kept (her health, speed and
    /// damage stay on its <see cref="BasicEnemy"/> fields). She is moved up Meadow's last wave only
    /// while she is still its last entry, where Build Bosses puts her.
    /// Art: the top-down White, Black and Shift sprites from
    /// <c>B:\Projects\GameDev\Sandbox\hth-art-3q\topdown\build_slide_art.py</c> (2026-10-16, scripted
    /// pixel art, no image model), copied into <see cref="ArtFolder"/>; the Shift strip is cut into
    /// its three in-between frames.
    /// </remarks>
    internal static class QueenMantisBuilder
    {
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string PrefabPath = EnemyPrefabFolder + "/Enemy_QueenMantis.prefab";
        private const string MantisSpawnPath = EnemyPrefabFolder + "/Enemy_MantisSpawn.prefab";
        private const string DataFolder = "Assets/_Game/Features/Enemies/Data";
        private const string DataPath = DataFolder + "/QueenMantisData.asset";
        private const string MeadowWavesPath = DataFolder + "/L01_Meadow_Waves.asset";
        private const string ArtFolder = "Assets/_Game/Features/Enemies/Art/QueenMantis";
        private const string WhitePath = ArtFolder + "/QueenMantisTopDown_White.png";
        private const string BlackPath = ArtFolder + "/QueenMantisTopDown_Black.png";
        private const string RingPath = "Assets/_Game/Shared/Art/Placeholders/Ring.png";
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";

        // Black to white, the in-between frames of the Shift strip.
        private static readonly string[] ShiftPaths =
        {
            ArtFolder + "/QueenMantisTopDown_Shift1.png",
            ArtFolder + "/QueenMantisTopDown_Shift2.png",
            ArtFolder + "/QueenMantisTopDown_Shift3.png",
        };

        // 384 x 576 pixels at 384 per unit: one grid cell wide, one and a half long.
        private const float ArtPixelsPerUnit = 384f;

        // The ring sprite: one world unit across.
        private const int RingSize = 96;
        private const int RingThickness = 5;

        // Bosses over the ordinary walkers (5), under the flyers (7); the pulse over the terrain (0), under the hill (4).
        private const int BodySortingOrder = 6;
        private const int PulseSortingOrder = 3;

        // Her placeholder numbers when the prefab is new (Alpha step 6).
        private const double NewHealth = 300.0;
        private const int NewDamage = 40;
        private const float NewSpeed = 0.8f;
        private const int NewBounty = 80;

        // In Meadow's last wave she walks in after the 4 Rolly Pollies and 4 Rhino Beetles, so the
        // wasps, Mantis Spawn, basics and scarabs behind her overtake her through her reach.
        private const int MeadowWaveSlot = 8;
        private const float GapAfterHer = 2f;

        // Build Bosses holds the entry before a boss back this long (AlphaBossesBuilder.GapBeforeBoss).
        private const float GapBeforeBoss = 3f;

        // The boss bar, top left (where the Honeydew line was until 2026-11-05): the top centre holds the wave label and
        // the Queen's own bar. The help box opens there only on Tutorial Hill, where she never comes.
        private static readonly Vector2 BarPosition = new Vector2(24f, -80f);
        private static readonly Vector2 BarSize = new Vector2(560f, 64f);
        private static readonly Color BarPanelColor = new Color(0f, 0f, 0f, 0.45f);
        private static readonly Color BarBackColor = new Color(0.1f, 0.1f, 0.12f, 0.95f);
        private static readonly Color ThirdMarkColor = new Color(0.05f, 0.05f, 0.05f, 1f);

        [MenuItem("Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar)")]
        private static void Build()
        {
            // From the menu, the open scenes are about to be replaced: offer to save them first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            BuildAll();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.QueenMantisBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildAll()
        {
            EnsurePrefab();
            PlaceInMeadow();
            AssetDatabase.SaveAssets();

            WireUiScene();
            ReportAfterReload();
            EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            Debug.Log("Queen Mantis built: in Play, F8 sends her down the road and F9 takes a third of her health.");
        }

        /// <summary>Art, ring sprite, data asset and prefab, with no scene opened. Build Bosses calls this too.</summary>
        internal static GameObject EnsurePrefab()
        {
            ImportArt();
            Sprite ring = EnsureRingSprite();
            QueenMantisData data = EnsureData(ring);
            AlphaCoreLoopBuilder.EnsureFolder(EnemyPrefabFolder);

            bool exists = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null;
            GameObject root = exists ? PrefabUtility.LoadPrefabContents(PrefabPath) : NewPrefabRoot();

            // Loaded again by path, in case opening the prefab let loose assets go (as OpenScene does).
            data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(DataPath);
            ring = AssetDatabase.LoadAssetAtPath<Sprite>(RingPath);
            try
            {
                Upgrade(root, data, ring);
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                if (exists)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
                else
                {
                    Object.DestroyImmediate(root);
                }
            }

            Debug.Log(exists ? "Enemy_QueenMantis upgraded in place." : "Made Enemy_QueenMantis.");
            return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        }

        // ---- Art and data ---------------------------------------------------------------------

        private static void ImportArt()
        {
            ImportPixelArt(WhitePath);
            ImportPixelArt(BlackPath);
            foreach (string path in ShiftPaths)
            {
                ImportPixelArt(path);
            }
        }

        // Pixel art: Point filter, no compression, no mipmaps, pixels per unit on purpose (UNITY_RULES 90).
        private static void ImportPixelArt(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError($"{path} is missing; copy it from the art Sandbox (see the builder's notes).");
                return;
            }

            if (importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Single
                && Mathf.Approximately(importer.spritePixelsPerUnit, ArtPixelsPerUnit)
                && importer.filterMode == FilterMode.Point
                && importer.textureCompression == TextureImporterCompression.Uncompressed
                && !importer.mipmapEnabled)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = ArtPixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        // A white ring one unit across, for the heal pulse and the body markers.
        private static Sprite EnsureRingSprite()
        {
            if (AssetDatabase.LoadAssetAtPath<Sprite>(RingPath) == null)
            {
                AlphaCoreLoopBuilder.EnsureFolder(Path.GetDirectoryName(RingPath).Replace('\\', '/'));
                var texture = new Texture2D(RingSize, RingSize, TextureFormat.RGBA32, false);
                var pixels = new Color32[RingSize * RingSize];
                float outer = RingSize * 0.5f;
                float inner = outer - RingThickness;
                for (int y = 0; y < RingSize; y++)
                {
                    for (int x = 0; x < RingSize; x++)
                    {
                        float dx = x + 0.5f - outer;
                        float dy = y + 0.5f - outer;
                        float distanceSquared = dx * dx + dy * dy;
                        bool onRing = distanceSquared <= outer * outer && distanceSquared >= inner * inner;
                        pixels[y * RingSize + x] = onRing ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                    }
                }
                texture.SetPixels32(pixels);
                texture.Apply();

                File.WriteAllBytes(RingPath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(RingPath);

                var importer = (TextureImporter)AssetImporter.GetAtPath(RingPath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = RingSize;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(RingPath);
        }

        private static QueenMantisData EnsureData(Sprite ring)
        {
            var data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(DataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<QueenMantisData>();
                AssetDatabase.CreateAsset(data, DataPath);
                Debug.Log("Made QueenMantisData.");
            }

            // Only empty references are filled, so a swap made in the Inspector stays.
            if (data.WhiteSprite == null)
            {
                data.WhiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WhitePath);
            }

            if (data.BlackSprite == null)
            {
                data.BlackSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BlackPath);
            }

            if (data.ShiftFrames == null || data.ShiftFrames.Length == 0)
            {
                data.ShiftFrames = new Sprite[ShiftPaths.Length];
                for (int i = 0; i < ShiftPaths.Length; i++)
                {
                    data.ShiftFrames[i] = AssetDatabase.LoadAssetAtPath<Sprite>(ShiftPaths[i]);
                }
            }

            if (data.RingSprite == null)
            {
                data.RingSprite = ring;
            }

            // The 10-20 review: a Queen Mantis built before the Mantis Spawn existed raised nothing for good.
            if (data.SpawnPrefab == null)
            {
                var spawn = AssetDatabase.LoadAssetAtPath<GameObject>(MantisSpawnPath);
                data.SpawnPrefab = spawn != null ? spawn.GetComponent<BasicEnemy>() : null;
                if (data.SpawnPrefab == null)
                {
                    Debug.LogWarning($"{MantisSpawnPath} does not exist yet; run Build Enemies, then this again. She summons and raises nothing until it is set.");
                }
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            return data;
        }

        // ---- Prefab ---------------------------------------------------------------------------

        private static GameObject NewPrefabRoot()
        {
            var go = new GameObject("Enemy_QueenMantis");
            var boss = go.AddComponent<QueenMantisBoss>();
            var so = new SerializedObject(boss);
            so.FindProperty("_maxHealth").doubleValue = NewHealth;
            so.FindProperty("_damage").intValue = NewDamage;
            so.FindProperty("_moveSpeed").floatValue = NewSpeed;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<RegisteredEnemy>();
            var bountySo = new SerializedObject(go.AddComponent<Bounty>());
            bountySo.FindProperty("_honeydew").intValue = NewBounty;
            bountySo.ApplyModifiedPropertiesWithoutUndo();
            return go;
        }

        private static void Upgrade(GameObject root, QueenMantisData data, Sprite ring)
        {
            var boss = root.GetComponent<QueenMantisBoss>();
            var so = new SerializedObject(boss);
            SetIfEmpty(so, "_data", data);
            // Her body turns to face the road (QueenMantisLook), so Jason's left-right flip is off.
            so.FindProperty("_flipSpriteFacing").boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Step 6's placeholder was a tinted circle on the root, scaled 1.1. Her body is a child
            // now, so it can turn without turning the pulse ring with it.
            var oldCircle = root.GetComponent<SpriteRenderer>();
            if (oldCircle != null)
            {
                Object.DestroyImmediate(oldCircle);
                root.transform.localScale = Vector3.one;
            }

            // Body first: BasicEnemy takes the first SpriteRenderer it finds as hers.
            SpriteRenderer body = EnsureChildRenderer(root.transform, "Body", 0, data.WhiteSprite, BodySortingOrder);
            SpriteRenderer pulseRenderer = EnsureChildRenderer(root.transform, "Heal Pulse", 1, ring, PulseSortingOrder);
            pulseRenderer.enabled = false;
            QueenMantisPulse pulse = EnsureComponent<QueenMantisPulse>(pulseRenderer.gameObject);

            SetIfEmpty(EnsureComponent<QueenMantisHealAura>(root), "_pulse", pulse);
            EnsureComponent<QueenMantisRaiser>(root);
            EnsureComponent<QueenMantisBodyMarkers>(root);
            EnsureComponent<QueenMantisSummoner>(root);
            SetIfEmpty(EnsureComponent<QueenMantisLook>(root), "_body", body);
            EnsureComponent<RegisteredEnemy>(root);
        }

        private static SpriteRenderer EnsureChildRenderer(Transform parent, string name, int siblingIndex, Sprite sprite, int sortingOrder)
        {
            Transform child = parent.Find(name);
            bool made = child == null;
            if (made)
            {
                child = new GameObject(name).transform;
                child.SetParent(parent, worldPositionStays: false);
            }
            child.SetSiblingIndex(siblingIndex);

            var renderer = child.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = child.gameObject.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = sortingOrder;
            }

            if (renderer.sprite == null)
            {
                renderer.sprite = sprite;
            }
            return renderer;
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }

        private static void SetIfEmpty(Component component, string field, Object value)
        {
            var so = new SerializedObject(component);
            SetIfEmpty(so, field, value);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetIfEmpty(SerializedObject so, string field, Object value)
        {
            SerializedProperty property = so.FindProperty(field);
            if (property.objectReferenceValue == null)
            {
                property.objectReferenceValue = value;
            }
        }

        // ---- Meadow's last wave ---------------------------------------------------------------

        private static void PlaceInMeadow()
        {
            var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(MeadowWavesPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (waves == null || prefab == null || waves.waves.Count == 0)
            {
                Debug.LogWarning($"{MeadowWavesPath} or the Queen Mantis prefab is missing; run Build Home and Levels and Build Enemies first.");
                return;
            }

            Wave last = waves.waves[waves.waves.Count - 1];
            int at = IndexOfQueenMantis(last);
            if (at < 0)
            {
                last.enemies.Insert(Mathf.Min(MeadowWaveSlot, last.enemies.Count),
                    new EnemySpawnEntry { enemyPrefab = prefab, delayBeforeNext = GapAfterHer });
                EditorUtility.SetDirty(waves);
                Debug.Log($"{waves.name}: Queen Mantis added to {last.waveName} at place {MeadowWaveSlot + 1}.");
                return;
            }

            // Anywhere but last, someone placed her by hand: keep it.
            if (at != last.enemies.Count - 1 || at <= MeadowWaveSlot)
            {
                Debug.Log($"{waves.name}: the Queen Mantis is at place {at + 1} of {last.waveName}; kept as it is.");
                return;
            }

            EnemySpawnEntry her = last.enemies[at];
            last.enemies.RemoveAt(at);

            // Build Bosses held the last ordinary enemy back a beat for her; give it its group's gap again.
            EnemySpawnEntry before = last.enemies[at - 1];
            if (Mathf.Approximately(before.delayBeforeNext, GapBeforeBoss) && at >= 2)
            {
                before.delayBeforeNext = last.enemies[at - 2].delayBeforeNext;
            }

            her.delayBeforeNext = GapAfterHer;
            last.enemies.Insert(MeadowWaveSlot, her);
            EditorUtility.SetDirty(waves);
            Debug.Log($"{waves.name}: the Queen Mantis moved from the end of {last.waveName} to place {MeadowWaveSlot + 1}.");
        }

        // By component, not prefab identity, so a variant of her counts too (10-20 review).
        private static int IndexOfQueenMantis(Wave wave)
        {
            for (int i = 0; i < wave.enemies.Count; i++)
            {
                GameObject prefab = wave.enemies[i].enemyPrefab;
                if (prefab != null && prefab.GetComponent<QueenMantisBoss>() != null)
                {
                    return i;
                }
            }
            return -1;
        }

        // ---- UI scene -------------------------------------------------------------------------

        private static void WireUiScene()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            EnsureBossBar(ui);

            // Loaded after OpenScene: opening a scene unloads loose assets loaded before it.
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
            if (keys == null)
            {
                Debug.LogWarning("The UI scene has no Dev Keys; run Add Dev Keys and Camera Fit, then this again, for F8.");
            }
            else
            {
                var so = new SerializedObject(keys);
                so.FindProperty("_queenMantisPrefab").objectReferenceValue = prefab != null ? prefab.GetComponent<BasicEnemy>() : null;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);
        }

        private static void EnsureBossBar(Scene ui)
        {
            if (AlphaRunFrameBuilder.FindInScene<QueenMantisBossBar>(ui) != null)
            {
                return;
            }

            HudPanel hudPanel = AlphaRunFrameBuilder.FindInScene<HudPanel>(ui);
            var hud = hudPanel != null ? new SerializedObject(hudPanel).FindProperty("_hud").objectReferenceValue as GameObject : null;
            if (hud == null)
            {
                Debug.LogError("The UI scene has no HUD; run Build Run Frame first. No boss bar made.");
                return;
            }

            int layer = hud.layer;
            var holder = new GameObject("Queen Mantis Bar", typeof(RectTransform));
            holder.layer = layer;
            var holderRect = (RectTransform)holder.transform;
            holderRect.SetParent(hud.transform, worldPositionStays: false);
            AlphaRunFrameBuilder.Anchor(holderRect, new Vector2(0f, 1f), BarPosition, BarSize);

            // Nothing here takes clicks: the bar sits over the top of the map.
            Image panel = AlphaRunFrameBuilder.MakeImage(holder.transform, "Panel", BarPanelColor, layer);
            AlphaRunFrameBuilder.Stretch(panel.rectTransform, Vector2.zero);
            panel.raycastTarget = false;

            TextMeshProUGUI label = AlphaRunFrameBuilder.MakeText(panel.transform, "Label",
                string.Format(UiStrings.MantisBarFormat, UiStrings.QueenMantis, UiStrings.MantisWhite), 24, TextAlignmentOptions.Center, layer);
            AlphaRunFrameBuilder.Anchor(label.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -2f), new Vector2(BarSize.x - 20f, 30f));

            Image back = AlphaRunFrameBuilder.MakeImage(panel.transform, "Bar", BarBackColor, layer);
            AlphaRunFrameBuilder.Anchor(back.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 6f), new Vector2(BarSize.x - 20f, 22f));
            back.sprite = AlphaRunFrameBuilder.BuiltinSprite("UI/Skin/Background.psd");
            back.type = Image.Type.Sliced;
            back.raycastTarget = false;

            Image fill = AlphaRunFrameBuilder.MakeImage(back.transform, "Fill", Color.white, layer);
            AlphaRunFrameBuilder.Stretch(fill.rectTransform, new Vector2(3f, 3f));
            fill.sprite = AlphaRunFrameBuilder.BuiltinSprite("UI/Skin/UISprite.psd");
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 1f;
            fill.raycastTarget = false;

            // Where she changes shade; drawn after the fill, so over it.
            for (int third = 1; third <= 2; third++)
            {
                Image mark = AlphaRunFrameBuilder.MakeImage(back.transform, $"Third {third}", ThirdMarkColor, layer);
                RectTransform rect = mark.rectTransform;
                rect.anchorMin = new Vector2(third / 3f, 0f);
                rect.anchorMax = new Vector2(third / 3f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(3f, 0f);
                mark.raycastTarget = false;
            }

            var bar = holder.AddComponent<QueenMantisBossBar>();
            var so = new SerializedObject(bar);
            so.FindProperty("_panel").objectReferenceValue = panel.gameObject;
            so.FindProperty("_fill").objectReferenceValue = fill;
            so.FindProperty("_label").objectReferenceValue = label;
            so.ApplyModifiedPropertiesWithoutUndo();

            panel.gameObject.SetActive(false);
            Debug.Log("Made the Queen Mantis boss bar at the top left.");
        }

        // ---- Report ---------------------------------------------------------------------------

        // Loads everything again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            var data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(DataPath);
            if (data == null)
            {
                Debug.LogError("QueenMantisData is missing after the build.");
            }
            else
            {
                Debug.Log($"QueenMantisData: pray {data.PraySeconds} s; heal {data.HealAmount} every {data.HealInterval} s within {data.HealRadius}; "
                          + $"raise up to {data.HusksPerRaise} every {data.RaiseInterval} s within {data.RaiseRadius}, wind-up {data.RaiseWindUp} s, "
                          + $"husks at {data.HuskHealthFraction} health, memory {data.DeathMemorySeconds} s; summons {data.SummonOnEntry} on entry, "
                          + $"{data.SummonOnShadeChange} per change, keep pace {data.EscortsKeepHerPace}; spawn {NameOf(data.SpawnPrefab)}, "
                          + $"white {NameOf(data.WhiteSprite)}, black {NameOf(data.BlackSprite)}, shift frames {CountSet(data.ShiftFrames)}, ring {NameOf(data.RingSprite)}");
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("Enemy_QueenMantis is missing after the build.");
            }
            else
            {
                var boss = prefab.GetComponent<QueenMantisBoss>();
                var aura = prefab.GetComponent<QueenMantisHealAura>();
                var look = prefab.GetComponent<QueenMantisLook>();
                Transform body = prefab.transform.Find("Body");
                var text = new StringBuilder("Enemy_QueenMantis: ");
                text.Append(boss != null ? $"health {boss.MaxHealth}, speed {boss.MoveSpeed}, damage {boss.Damage}, data {AlphaRunFrameBuilder.Name(new SerializedObject(boss), "_data")}" : "NO QueenMantisBoss");
                text.Append(aura != null ? $"; heal aura, pulse {AlphaRunFrameBuilder.Name(new SerializedObject(aura), "_pulse")}" : "; NO heal aura");
                text.Append(prefab.GetComponent<QueenMantisRaiser>() != null ? "; raiser" : "; NO raiser");
                text.Append(prefab.GetComponent<QueenMantisBodyMarkers>() != null ? "; body markers" : "; NO body markers");
                text.Append(prefab.GetComponent<QueenMantisSummoner>() != null ? "; summoner" : "; NO summoner");
                text.Append(look != null ? $"; look, body {AlphaRunFrameBuilder.Name(new SerializedObject(look), "_body")}" : "; NO look");
                text.Append(prefab.GetComponent<RegisteredEnemy>() != null ? "; registered" : "; NOT registered");
                text.Append(prefab.GetComponent<Bounty>() != null ? $"; bounty {prefab.GetComponent<Bounty>().Amount}" : "; NO bounty");
                text.Append(prefab.GetComponent<SpriteRenderer>() == null ? "; no root sprite" : "; OLD ROOT SPRITE STILL THERE");
                text.Append(body != null ? $"; body child {body.GetSiblingIndex()} first, sprite {NameOf(body.GetComponent<SpriteRenderer>() != null ? body.GetComponent<SpriteRenderer>().sprite : null)}" : "; NO body child");
                text.Append($"; scale {prefab.transform.localScale.x}");
                Debug.Log(text.ToString());
            }

            var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(MeadowWavesPath);
            if (waves != null && waves.waves.Count > 0)
            {
                Wave last = waves.waves[waves.waves.Count - 1];
                Debug.Log($"{waves.name}: {last.waveName} in order = {InOrder(last)}");
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            QueenMantisBossBar bar = AlphaRunFrameBuilder.FindInScene<QueenMantisBossBar>(ui);
            if (bar == null)
            {
                Debug.LogError("The Queen Mantis boss bar is missing from the UI scene.");
            }
            else
            {
                var so = new SerializedObject(bar);
                Debug.Log($"Boss bar: under {bar.transform.parent.name}, panel = {AlphaRunFrameBuilder.Name(so, "_panel")}, "
                          + $"fill = {AlphaRunFrameBuilder.Name(so, "_fill")}, label = {AlphaRunFrameBuilder.Name(so, "_label")}");
            }

            DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
            Debug.Log($"Dev keys: F8 sends {(keys != null ? AlphaRunFrameBuilder.Name(new SerializedObject(keys), "_queenMantisPrefab") : "nothing (no Dev Keys)")}");
        }

        // The wave's entries in order, runs of one prefab folded: "Enemy_Wasp x5, Enemy_QueenMantis, ...".
        private static string InOrder(Wave wave)
        {
            var text = new StringBuilder();
            int i = 0;
            while (i < wave.enemies.Count)
            {
                GameObject prefab = wave.enemies[i].enemyPrefab;
                int run = 1;
                while (i + run < wave.enemies.Count && wave.enemies[i + run].enemyPrefab == prefab)
                {
                    run++;
                }

                if (text.Length > 0)
                {
                    text.Append(", ");
                }
                text.Append(prefab != null ? prefab.name : "none");
                if (run > 1)
                {
                    text.Append(" x").Append(run);
                }
                i += run;
            }
            return text.ToString();
        }

        private static string NameOf(Object value)
        {
            return value != null ? value.name : "none";
        }

        private static int CountSet(Sprite[] sprites)
        {
            int count = 0;
            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (sprites[i] != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
