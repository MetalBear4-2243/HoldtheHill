using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha sprites (2026-11-03): Nilly's top-down pixel sheets on the placeholder enemies and
    /// towers, each playing one looping row through a <see cref="SpriteFlipbook"/> (no Animator).
    /// Enemies loop Walk on a child "Body" that a <see cref="FaceTravel"/> turns to face the way they
    /// walk; towers loop Idle on their own sprite. The Queen Mantis keeps her own art and is not touched.
    /// Shots and the Carpenter's mine loop their row on their own sprite (shots turn to face their
    /// flight), and each leaves a one-shot effect (<see cref="SpriteOneShot"/>) where it hits.
    /// </summary>
    /// <remarks>
    /// Safe to run again: a sheet already sliced and set up is kept as it is, and the prefabs are
    /// upgraded in place (sprite, frames, body), so their numbers and other tuning stay.
    /// Sheets: the PNG and Aseprite JSON from <c>_Sandbox/nilly-ctrl/Animations/Sheets</c> on
    /// <c>nilly/c1-player-build</c> (CritterPillbug from <c>Sheets/Props</c> in
    /// <c>B:\Archive\hold-the-hill-drive-offload-2026-10-07.zip</c>), copied into the features' <c>Art/Sheets</c>.
    /// Each sheet is cut by its JSON, one sprite per frame, named as in Aseprite ("EnemyRunner Walk 0").
    /// Tower picks are Nilly's (2026-11-03): the six Alpha towers wear old-roster sheets.
    /// Shot picks are Nilly's (2026-11-04): the Bombardier shell, the old mine, and the Blast and mine
    /// Explode rows for the hits; on 2026-11-05 the Bullet Ant's shot became the stinger dart and the
    /// tower roles (AlphaTowerRolesBuilder) added the honey glob and its puddle, the flame puffs and the
    /// sap and poison overlays (weapon-picker and Fx sheets from the same Sandbox).
    /// To take the art off again: delete the two Art/Sheets folders and the Fx_ prefabs, and revert the prefabs.
    /// </remarks>
    internal static class AlphaSpritesBuilder
    {
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string TowerPrefabFolder = "Assets/_Game/Features/Towers/Prefabs";
        private const string EnemySheetFolder = "Assets/_Game/Features/Enemies/Art/Sheets";
        private const string TowerSheetFolder = "Assets/_Game/Features/Towers/Art/Sheets";
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";
        private const string BodyName = "Body";

        // As the placeholders: a 32 px frame is one world unit, one grid cell (TowerPlacer._cellSize is 1).
        // A tower's root is 0.8 and grows each upgrade (Tower._scalePerLevel), so it stays inside its cell.
        private const float PixelsPerUnit = 32f;

        private readonly struct Look
        {
            public readonly string Prefab;
            public readonly string Sheet;
            public readonly string Loop;
            public readonly bool VariantOfBasic;

            public Look(string prefab, string sheet, string loop, bool variantOfBasic = false)
            {
                Prefab = prefab;
                Sheet = sheet;
                Loop = loop;
                VariantOfBasic = variantOfBasic;
            }
        }

        // Enemy_Basic first: Scarab and Mantis Spawn are its variants and inherit its Body.
        // The Rhino Beetle's Walk row has the shield bubble drawn in; its prefab already shows the
        // shield as a child that fades when the shield breaks, so the body loops WalkBare.
        private static readonly Look[] Enemies =
        {
            new Look("Enemy_Basic", "EnemyGrunt", "Walk"),
            new Look("Enemy_Scarab", "EnemyRunner", "Walk", variantOfBasic: true),
            new Look("Enemy_MantisSpawn", "EnemyHealer", "Walk", variantOfBasic: true),
            new Look("Enemy_RhinoBeetle", "EnemyShielded", "WalkBare"),
            new Look("Enemy_Wasp", "EnemyWasp", "Walk"),
            new Look("Enemy_RollyPolly", "CritterPillbug", "Walk"),
            new Look("Enemy_Silverfish", "EnemySilverfish", "Walk"),
            new Look("Enemy_Centipede", "EnemySplitter", "Walk"),
        };

        private static readonly Look[] Towers =
        {
            new Look("Tower_Ant", "TowerSoldier", "Idle"),
            new Look("Tower_BulletAnt", "TowerLinear", "Idle"),
            new Look("Tower_CrazyAnt", "TowerKnockback", "Idle"),
            new Look("Tower_FireAnt", "TowerMortar", "Idle"),
            new Look("Tower_SapAnt", "TowerFrostAura", "Idle"),
            new Look("Tower_CarpenterAnt", "TowerMineLayer", "Idle"),
        };

        // A shot (or a mine) and the effect it leaves where it hits. Every sheet faces right.
        private readonly struct ShotLook
        {
            public readonly string Prefab;
            public readonly string Sheet;
            public readonly string Loop;
            public readonly bool FacesFlight;
            public readonly string EffectField;
            public readonly string Effect;
            public readonly string EffectSheet;
            public readonly string EffectRow;

            public ShotLook(string prefab, string sheet, string loop, bool facesFlight,
                string effectField, string effect, string effectSheet, string effectRow)
            {
                Prefab = prefab;
                Sheet = sheet;
                Loop = loop;
                FacesFlight = facesFlight;
                EffectField = effectField;
                Effect = effect;
                EffectSheet = effectSheet;
                EffectRow = effectRow;
            }
        }

        // The Bullet Ant's fast bullet is the stinger dart (2026-11-05). The Sap Ant's glob leaves its
        // puddle (Fx_SapPuddle, made and wired by AlphaTowerRolesBuilder), so it has no effect here.
        // Projectile_Lob is no tower's shot since the Fire Ant became a flamethrower; it keeps its art.
        private static readonly ShotLook[] Shots =
        {
            new ShotLook("Projectile_Bullet", "WpnStingerDart", "Fly", facesFlight: true, "_hitEffect", "Fx_BulletHit", "WpnStingerDart", "Hit"),
            new ShotLook("Projectile_Lob", "ProjMortar", "Fly", facesFlight: true, "_hitEffect", "Fx_LobBlast", "FxBlast", "Blast"),
            new ShotLook("Mine_Carpenter", "FxMine", "Armed", facesFlight: false, "_explosionEffect", "Fx_MineBlast", "FxMine", "Explode"),
            new ShotLook("Projectile_Sap", "WpnHoneyGlob", "Fly", facesFlight: true, null, null, null, null),
        };

        // Which frame of a row a prefab's own sprite shows in the Editor (the component plays the rest).
        private enum StillFrame
        {
            None,
            First,
            Last,
        }

        // A row of frames for a field pair on one of the tower roles' parts (AlphaTowerRolesBuilder
        // makes them): the sap puddle's splat and the overlay it puts on stuck enemies, the flame
        // puff's loop, the Crazy Ant's poison overlay.
        private readonly struct RowSlot
        {
            public readonly string Prefab;
            public readonly string Component;
            public readonly string FramesField;
            public readonly string SecondsField;
            public readonly string Sheet;
            public readonly string Row;
            public readonly StillFrame Still;

            public RowSlot(string prefab, string component, string framesField, string secondsField, string sheet, string row, StillFrame still)
            {
                Prefab = prefab;
                Component = component;
                FramesField = framesField;
                SecondsField = secondsField;
                Sheet = sheet;
                Row = row;
                Still = still;
            }
        }

        private static readonly RowSlot[] Extras =
        {
            new RowSlot("Fx_SapPuddle", nameof(SapPuddle), "_splatFrames", "_splatFrameSeconds", "WpnHoneyGlob", "Hit", StillFrame.Last),
            new RowSlot("Fx_SapPuddle", nameof(SapPuddle), "_overlayFrames", "_overlayFrameSeconds", "FxStatusOverlay", "Honey", StillFrame.None),
            new RowSlot("Fx_FlamePuff", nameof(SpriteFlipbook), "_frames", "_frameSeconds", "WpnFlameBurst", "Fly", StillFrame.First),
            new RowSlot("Tower_CrazyAnt", nameof(PoisonOnHits), "_overlayFrames", "_overlayFrameSeconds", "FxStatusOverlay", "Poison", StillFrame.None),
        };

        // Shots, the mine and their effects draw at the towers' pixel size: a sheet pixel is 1/40 unit,
        // as on a tower (root 0.8) and the Basic enemy (0.6 x 32/24). The glob, shell and mine then come
        // out about as big as the circles they replace. Set on the first upgrade only; then it is the Inspector's.
        private const float ShotScale = 0.8f;

        // A shot points along its flight at once (a short lob turns half round in a quarter second).
        private const float ShotTurnDegreesPerSecond = 3600f;

        // Over the shots (20) and the enemies, so a splash shows on the enemy it hit.
        private const int EffectSortingOrder = 21;

        [MenuItem("Tools/Hold the Hill/Alpha/Build Sprites (enemy, tower and shot sheets, looping)")]
        private static void Build()
        {
            BuildAll();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaSpritesBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildAll()
        {
            foreach (Look look in Enemies)
            {
                ImportSheet(SheetPath(EnemySheetFolder, look.Sheet));
            }
            foreach (Look look in Towers)
            {
                ImportSheet(SheetPath(TowerSheetFolder, look.Sheet));
            }
            var shotSheets = new List<string>();
            foreach (ShotLook shot in Shots)
            {
                foreach (string sheet in new[] { shot.Sheet, shot.EffectSheet })
                {
                    if (sheet != null && !shotSheets.Contains(sheet))
                    {
                        shotSheets.Add(sheet);
                    }
                }
            }
            foreach (RowSlot slot in Extras)
            {
                if (!shotSheets.Contains(slot.Sheet))
                {
                    shotSheets.Add(slot.Sheet);
                }
            }
            foreach (string sheet in shotSheets)
            {
                ImportSheet(SheetPath(TowerSheetFolder, sheet));
            }

            foreach (Look look in Enemies)
            {
                UpgradePrefab(EnemyPrefabFolder, EnemySheetFolder, look, isEnemy: true);
            }
            foreach (Look look in Towers)
            {
                UpgradePrefab(TowerPrefabFolder, TowerSheetFolder, look, isEnemy: false);
                SetShopIcon(look);
            }

            // Effects first: a shot points at its effect's prefab.
            foreach (ShotLook shot in Shots)
            {
                if (shot.Effect != null)
                {
                    EnsureEffect(shot);
                }
            }
            foreach (ShotLook shot in Shots)
            {
                UpgradeShot(shot);
            }
            foreach (RowSlot slot in Extras)
            {
                SetRowSlot(slot);
            }

            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Alpha sprites built: enemies loop their walk, face the road, flinch and die; towers loop their idle and play their attack; shop cards show the tower; shots fly with their art and leave a splash.");
        }

        private static string SheetPath(string folder, string sheet) => $"{folder}/{sheet}.png";

        private static string PrefabPath(string folder, string prefab) => $"{folder}/{prefab}.prefab";

        // ---- Sheets ---------------------------------------------------------------------------

        [Serializable]
        private class AsepriteSheet
        {
            public AsepriteFrame[] frames;
        }

        [Serializable]
        private class AsepriteFrame
        {
            public string filename;
            public AsepriteRect frame;
            public int duration;
        }

        [Serializable]
        private class AsepriteRect
        {
            public int x;
            public int y;
            public int w;
            public int h;
        }

        private static AsepriteSheet ReadSheet(string pngPath)
        {
            string jsonPath = pngPath.Substring(0, pngPath.Length - ".png".Length) + ".json";
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
            if (json == null)
            {
                Debug.LogError($"{jsonPath} is missing; copy it beside its sheet (see the builder's notes).");
                return null;
            }
            return JsonUtility.FromJson<AsepriteSheet>(json.text);
        }

        // Pixel art: Point filter, no compression, no mipmaps, 32 per unit (UNITY_RULES 90); one sprite per frame.
        private static void ImportSheet(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            AsepriteSheet sheet = ReadSheet(path);
            if (importer == null || sheet == null || sheet.frames == null || sheet.frames.Length == 0)
            {
                Debug.LogError($"{path} is missing or has no frames; copy it from the art Sandbox (see the builder's notes).");
                return;
            }

            bool settingsChanged = false;
            if (importer.textureType != TextureImporterType.Sprite
                || importer.spriteImportMode != SpriteImportMode.Multiple
                || !Mathf.Approximately(importer.spritePixelsPerUnit, PixelsPerUnit)
                || importer.filterMode != FilterMode.Point
                || importer.textureCompression != TextureImporterCompression.Uncompressed
                || importer.mipmapEnabled
                || !importer.alphaIsTransparency)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritePixelsPerUnit = PixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
                settingsChanged = true;
            }

            // The data provider reads the import mode when it is made, so it comes after the settings.
            importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            ISpriteEditorDataProvider provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();

            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            SpriteRect[] current = provider.GetSpriteRects();
            SpriteRect[] wanted = FrameRects(path, sheet, height, current);
            if (SameRects(current, wanted))
            {
                Debug.Log($"{path}: {wanted.Length} frames, {width}x{height}; {(settingsChanged ? "import settings set" : "kept as it is")}.");
                return;
            }

            provider.SetSpriteRects(wanted);
            provider.Apply();
            importer.SaveAndReimport();
            Debug.Log($"{path}: cut into {wanted.Length} frames by its JSON.");
        }

        // Aseprite counts y from the top, Unity from the bottom. A frame that already exists keeps its
        // id, so references to it survive a re-cut; a new one gets an id made from its name.
        private static SpriteRect[] FrameRects(string path, AsepriteSheet sheet, int textureHeight, SpriteRect[] current)
        {
            var existing = new Dictionary<string, GUID>();
            foreach (SpriteRect rect in current)
            {
                if (!string.IsNullOrEmpty(rect.name) && !existing.ContainsKey(rect.name))
                {
                    existing.Add(rect.name, rect.spriteID);
                }
            }

            var rects = new SpriteRect[sheet.frames.Length];
            for (int i = 0; i < sheet.frames.Length; i++)
            {
                AsepriteFrame frame = sheet.frames[i];
                AsepriteRect r = frame.frame;
                rects[i] = new SpriteRect
                {
                    name = frame.filename,
                    rect = new Rect(r.x, textureHeight - r.y - r.h, r.w, r.h),
                    alignment = SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f),
                    border = Vector4.zero,
                    spriteID = existing.TryGetValue(frame.filename, out GUID id) ? id : StableId(path + "/" + frame.filename),
                };
            }
            return rects;
        }

        private static bool SameRects(SpriteRect[] current, SpriteRect[] wanted)
        {
            if (current.Length != wanted.Length)
            {
                return false;
            }

            for (int i = 0; i < wanted.Length; i++)
            {
                SpriteRect match = Array.Find(current, c => c.name == wanted[i].name);
                if (match == null || match.rect != wanted[i].rect || match.alignment != SpriteAlignment.Center)
                {
                    return false;
                }
            }
            return true;
        }

        private static GUID StableId(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                var hex = new StringBuilder(32);
                foreach (byte b in hash)
                {
                    hex.Append(b.ToString("x2"));
                }
                return new GUID(hex.ToString());
            }
        }

        private readonly struct Loop
        {
            public readonly Sprite[] Frames;
            public readonly float FrameSeconds;
            public readonly int FrameWidth;

            public Loop(Sprite[] frames, float frameSeconds, int frameWidth)
            {
                Frames = frames;
                FrameSeconds = frameSeconds;
                FrameWidth = frameWidth;
            }

            public bool IsEmpty => Frames == null || Frames.Length == 0;
        }

        // The tag's frames in sheet order, with the sheet's own frame time.
        private static Loop LoadLoop(string path, string tag)
        {
            AsepriteSheet sheet = ReadSheet(path);
            if (sheet == null)
            {
                return default;
            }

            var byName = new Dictionary<string, Sprite>();
            foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (asset is Sprite sprite)
                {
                    byName[sprite.name] = sprite;
                }
            }

            var frames = new List<Sprite>();
            float seconds = 0.1f;
            int width = 0;
            foreach (AsepriteFrame frame in sheet.frames)
            {
                string[] parts = frame.filename.Split(' ');
                if (parts.Length < 2 || parts[1] != tag)
                {
                    continue;
                }

                if (!byName.TryGetValue(frame.filename, out Sprite sprite))
                {
                    Debug.LogError($"{path} has no sprite named \"{frame.filename}\"; the sheet was not cut.");
                    return default;
                }

                if (frames.Count == 0)
                {
                    seconds = Mathf.Max(0.01f, frame.duration / 1000f);
                    width = frame.frame.w;
                }
                frames.Add(sprite);
            }

            if (frames.Count == 0)
            {
                Debug.LogError($"{path} has no \"{tag}\" frames.");
            }
            return new Loop(frames.ToArray(), seconds, width);
        }

        // ---- Shop icons -----------------------------------------------------------------------

        // The tower's shop card (Adam's TowerList) shows its first idle frame: the same picture as the
        // tower standing on the map. Tower_Ant's data is Features/Towers/Data/Ant.asset, and so on.
        private static void SetShopIcon(Look look)
        {
            const string PrefabPrefix = "Tower_";
            string dataName = look.Prefab.StartsWith(PrefabPrefix, StringComparison.Ordinal)
                ? look.Prefab.Substring(PrefabPrefix.Length)
                : look.Prefab;
            string dataPath = $"{TowerDataFolder}/{dataName}.asset";

            var data = AssetDatabase.LoadAssetAtPath<TowerData>(dataPath);
            if (data == null)
            {
                Debug.LogError($"{dataPath} is missing, so {look.Prefab}'s shop card keeps its old icon.");
                return;
            }

            Loop loop = LoadLoop(SheetPath(TowerSheetFolder, look.Sheet), look.Loop);
            if (loop.IsEmpty || data.towerIcon == loop.Frames[0])
            {
                return;
            }

            data.towerIcon = loop.Frames[0];
            EditorUtility.SetDirty(data);
        }

        // ---- Prefabs --------------------------------------------------------------------------

        private static void UpgradePrefab(string prefabFolder, string sheetFolder, Look look, bool isEnemy)
        {
            string prefabPath = PrefabPath(prefabFolder, look.Prefab);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            {
                Debug.LogError($"{prefabPath} is missing; run the Alpha builders that make it first.");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                // Loaded after the prefab, in case opening it let loose assets go (as OpenScene does).
                Loop loop = LoadLoop(SheetPath(sheetFolder, look.Sheet), look.Loop);
                if (loop.IsEmpty)
                {
                    return;
                }

                bool upgraded = isEnemy ? UpgradeEnemy(root, look, loop) : UpgradeTower(root, look, loop);
                if (upgraded)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        // The placeholder circle on the root goes; the art is drawn by a child Body that can turn
        // without turning the root's colliders and children with it (as the Queen Mantis does).
        private static bool UpgradeEnemy(GameObject root, Look look, Loop loop)
        {
            var enemy = root.GetComponent<BasicEnemy>();
            if (enemy == null)
            {
                Debug.LogError($"{look.Prefab} has no BasicEnemy; left as it is.");
                return false;
            }

            // The body turns to face the road, so Jason's left-right flip is off.
            var enemySo = new SerializedObject(enemy);
            SerializedProperty flip = enemySo.FindProperty("_flipSpriteFacing");
            if (flip.boolValue)
            {
                flip.boolValue = false;
                enemySo.ApplyModifiedPropertiesWithoutUndo();
            }

            var circle = root.GetComponent<SpriteRenderer>();
            Transform body = root.transform.Find(BodyName);
            if (body == null)
            {
                if (look.VariantOfBasic)
                {
                    Debug.LogError($"{look.Prefab} is a variant of Enemy_Basic but has no Body; upgrade Enemy_Basic first.");
                    return false;
                }

                body = new GameObject(BodyName).transform;
                body.SetParent(root.transform, worldPositionStays: false);
                var made = body.gameObject.AddComponent<SpriteRenderer>();
                if (circle != null)
                {
                    made.sharedMaterial = circle.sharedMaterial;
                    made.sortingLayerID = circle.sortingLayerID;
                    made.sortingOrder = circle.sortingOrder;
                }
            }

            // Body first: BasicEnemy (and the curl and husk tints) take the first SpriteRenderer they find.
            if (body.GetSiblingIndex() != 0)
            {
                body.SetSiblingIndex(0);
            }

            if (circle != null)
            {
                Object.DestroyImmediate(circle);
            }

            var renderer = body.GetComponent<SpriteRenderer>();
            renderer.sprite = loop.Frames[0];
            renderer.color = Color.white;
            // The frame fills the placeholder circle it replaces (one unit before the root's scale).
            body.localScale = Vector3.one * (PixelsPerUnit / loop.FrameWidth);
            body.localPosition = Vector3.zero;

            SpriteFlipbook walk = EnsureComponent<SpriteFlipbook>(body.gameObject);
            SetFlipbook(walk, renderer, loop);
            SetHurtAndDeath(root, look, renderer, walk);

            var face = new SerializedObject(EnsureComponent<FaceTravel>(root));
            face.FindProperty("_body").objectReferenceValue = body;
            // Every enemy sheet faces right.
            face.FindProperty("_artFacingDegrees").floatValue = 0f;
            face.ApplyModifiedPropertiesWithoutUndo();

            // A variant's old colour override pointed at Enemy_Basic's circle, which is gone.
            if (look.VariantOfBasic)
            {
                PrefabUtility.RemoveUnusedOverrides(new[] { root }, InteractionMode.AutomatedAction);
            }
            return true;
        }

        // The sheet's Hurt row plays over the walk on each hit, its Death row where the enemy dies
        // (EnemyHurtAndDeath). A sheet without them (the pill bug) leaves the enemy as it was.
        private const string HurtRow = "Hurt";
        private const string DeathRow = "Death";

        private static void SetHurtAndDeath(GameObject root, Look look, SpriteRenderer body, SpriteFlipbook walk)
        {
            string path = SheetPath(EnemySheetFolder, look.Sheet);
            Loop hurt = SheetHasRow(path, HurtRow) ? LoadLoop(path, HurtRow) : default;
            Loop death = SheetHasRow(path, DeathRow) ? LoadLoop(path, DeathRow) : default;
            if (hurt.IsEmpty && death.IsEmpty)
            {
                return;
            }

            var so = new SerializedObject(EnsureComponent<EnemyHurtAndDeath>(root));
            so.FindProperty("_body").objectReferenceValue = body;
            so.FindProperty("_walk").objectReferenceValue = walk;
            SetRow(so, "_hurtFrames", "_hurtFrameSeconds", hurt);
            SetRow(so, "_deathFrames", "_deathFrameSeconds", death);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetRow(SerializedObject so, string framesField, string secondsField, Loop row)
        {
            Sprite[] frames = row.IsEmpty ? Array.Empty<Sprite>() : row.Frames;
            SerializedProperty list = so.FindProperty(framesField);
            list.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            }

            if (!row.IsEmpty)
            {
                so.FindProperty(secondsField).floatValue = row.FrameSeconds;
            }
        }

        private static bool SheetHasRow(string pngPath, string row)
        {
            AsepriteSheet sheet = ReadSheet(pngPath);
            if (sheet == null || sheet.frames == null)
            {
                return false;
            }

            foreach (AsepriteFrame frame in sheet.frames)
            {
                string[] parts = frame.filename.Split(' ');
                if (parts.Length >= 2 && parts[1] == row)
                {
                    return true;
                }
            }

            return false;
        }

        // A tower does not turn, so its own sprite plays the loop. Tower keeps the base colour it reads
        // in Awake for its stun and upgrade tints, so the placeholder colour goes back to white.
        private static bool UpgradeTower(GameObject root, Look look, Loop loop)
        {
            var renderer = root.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogError($"{root.name} has no SpriteRenderer on its root; left as it is.");
                return false;
            }

            renderer.sprite = loop.Frames[0];
            renderer.color = Color.white;
            SpriteFlipbook idle = EnsureComponent<SpriteFlipbook>(root);
            SetFlipbook(idle, renderer, loop);
            SetAttack(root, look, renderer, idle);
            return true;
        }

        // The sheet's Attack row plays over the idle loop when the tower attacks (TowerAttackAnimation):
        // frames 0-1 wind up, frame 2 lands the blow, as the Assets/Ants animations are drawn.
        private const string AttackRow = "Attack";
        private const int AttackHitFrame = 2;

        private static void SetAttack(GameObject root, Look look, SpriteRenderer renderer, SpriteFlipbook idle)
        {
            Loop attack = LoadLoop(SheetPath(TowerSheetFolder, look.Sheet), AttackRow);
            if (attack.IsEmpty)
            {
                Debug.LogError($"{look.Sheet} has no {AttackRow} row, so {look.Prefab} keeps only its idle.");
                return;
            }

            var so = new SerializedObject(EnsureComponent<TowerAttackAnimation>(root));
            so.FindProperty("_renderer").objectReferenceValue = renderer;
            so.FindProperty("_idle").objectReferenceValue = idle;
            so.FindProperty("_frameSeconds").floatValue = attack.FrameSeconds;
            so.FindProperty("_hitFrame").intValue = Mathf.Min(AttackHitFrame, attack.Frames.Length - 1);
            SerializedProperty frames = so.FindProperty("_frames");
            frames.arraySize = attack.Frames.Length;
            for (int i = 0; i < attack.Frames.Length; i++)
            {
                frames.GetArrayElementAtIndex(i).objectReferenceValue = attack.Frames[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFlipbook(SpriteFlipbook flipbook, SpriteRenderer renderer, Loop loop)
        {
            var so = new SerializedObject(flipbook);
            so.FindProperty("_renderer").objectReferenceValue = renderer;
            so.FindProperty("_frameSeconds").floatValue = loop.FrameSeconds;
            SerializedProperty frames = so.FindProperty("_frames");
            frames.arraySize = loop.Frames.Length;
            for (int i = 0; i < loop.Frames.Length; i++)
            {
                frames.GetArrayElementAtIndex(i).objectReferenceValue = loop.Frames[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---- Shots and hit effects ------------------------------------------------------------

        // A tiny prefab per effect: a SpriteRenderer and a SpriteOneShot playing the row once.
        // Made the first time; after that only the frames are refreshed, so scale and order stay as tuned.
        private static void EnsureEffect(ShotLook look)
        {
            string path = PrefabPath(TowerPrefabFolder, look.Effect);
            bool isNew = AssetDatabase.LoadAssetAtPath<GameObject>(path) == null;
            GameObject root = isNew ? new GameObject(look.Effect) : PrefabUtility.LoadPrefabContents(path);
            try
            {
                Loop row = LoadLoop(SheetPath(TowerSheetFolder, look.EffectSheet), look.EffectRow);
                if (row.IsEmpty)
                {
                    return;
                }

                var renderer = EnsureComponent<SpriteRenderer>(root);
                if (isNew)
                {
                    root.transform.localScale = Vector3.one * ShotScale;
                    // The shot's own material and layer, so the splash is lit as the shot is.
                    var shot = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, look.Prefab));
                    var shotRenderer = shot != null ? shot.GetComponent<SpriteRenderer>() : null;
                    if (shotRenderer != null)
                    {
                        renderer.sharedMaterial = shotRenderer.sharedMaterial;
                        renderer.sortingLayerID = shotRenderer.sortingLayerID;
                    }
                    renderer.sortingOrder = EffectSortingOrder;
                }

                renderer.sprite = row.Frames[0];
                renderer.color = Color.white;

                var so = new SerializedObject(EnsureComponent<SpriteOneShot>(root));
                so.FindProperty("_renderer").objectReferenceValue = renderer;
                so.FindProperty("_frameSeconds").floatValue = row.FrameSeconds;
                SerializedProperty frames = so.FindProperty("_frames");
                frames.arraySize = row.Frames.Length;
                for (int i = 0; i < row.Frames.Length; i++)
                {
                    frames.GetArrayElementAtIndex(i).objectReferenceValue = row.Frames[i];
                }
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                if (isNew)
                {
                    Object.DestroyImmediate(root);
                }
                else
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        // The shot keeps its root sprite and plays the loop there; a shot turns its whole self to its
        // flight (it has no children or colliders to keep still). The placeholder colour goes to white.
        private static void UpgradeShot(ShotLook look)
        {
            string prefabPath = PrefabPath(TowerPrefabFolder, look.Prefab);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            {
                Debug.LogError($"{prefabPath} is missing; run the builder that makes it first (Alpha core loop, or Tower Roles for the sap shot).");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                // Loaded after the prefab, in case opening it let loose assets go (as OpenScene does).
                string sheetPath = SheetPath(TowerSheetFolder, look.Sheet);
                Loop loop = LoadLoop(sheetPath, look.Loop);
                var effect = look.Effect != null ? AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, look.Effect)) : null;
                var renderer = root.GetComponent<SpriteRenderer>();
                if (loop.IsEmpty || renderer == null)
                {
                    Debug.LogError($"{look.Prefab}: no {look.Loop} frames or no SpriteRenderer on its root; left as it is.");
                    return;
                }

                // Not yet this sheet (the placeholder circle, or an earlier pick): take the towers' pixel
                // size once. On a rerun the sprite is from this sheet, so a tuned scale stays.
                if (AssetDatabase.GetAssetPath(renderer.sprite) != sheetPath)
                {
                    root.transform.localScale = Vector3.one * ShotScale;
                }

                renderer.sprite = loop.Frames[0];
                renderer.color = Color.white;
                SetFlipbook(EnsureComponent<SpriteFlipbook>(root), renderer, loop);

                if (look.FacesFlight)
                {
                    var face = new SerializedObject(EnsureComponent<FaceTravel>(root));
                    face.FindProperty("_body").objectReferenceValue = root.transform;
                    face.FindProperty("_artFacingDegrees").floatValue = 0f;
                    face.FindProperty("_turnDegreesPerSecond").floatValue = ShotTurnDegreesPerSecond;
                    face.ApplyModifiedPropertiesWithoutUndo();
                }

                if (look.Effect != null)
                {
                    MonoBehaviour owner = root.GetComponent<Projectile>();
                    if (owner == null)
                    {
                        owner = root.GetComponent<Mine>();
                    }
                    SerializedObject ownerSo = owner != null ? new SerializedObject(owner) : null;
                    SerializedProperty effectField = ownerSo?.FindProperty(look.EffectField);
                    if (effectField == null || effect == null)
                    {
                        Debug.LogError($"{look.Prefab}: no {look.EffectField} field or no {look.Effect} prefab; it flies without a hit effect.");
                    }
                    else
                    {
                        effectField.objectReferenceValue = effect;
                        ownerSo.ApplyModifiedPropertiesWithoutUndo();
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        // Fills one frames-and-seconds field pair from a sheet row; the prefab and its component are
        // the tower roles builder's. The prefab's own sprite shows the row's first or last frame.
        private static void SetRowSlot(RowSlot slot)
        {
            string prefabPath = PrefabPath(TowerPrefabFolder, slot.Prefab);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
            {
                Debug.LogError($"{prefabPath} is missing; run Build Tower Roles, which makes it.");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                Loop row = LoadLoop(SheetPath(TowerSheetFolder, slot.Sheet), slot.Row);
                Component component = root.GetComponent(slot.Component);
                if (row.IsEmpty || component == null)
                {
                    Debug.LogError($"{slot.Prefab}: no {slot.Sheet} {slot.Row} frames or no {slot.Component}; left as it is.");
                    return;
                }

                var so = new SerializedObject(component);
                SerializedProperty frames = so.FindProperty(slot.FramesField);
                frames.arraySize = row.Frames.Length;
                for (int i = 0; i < row.Frames.Length; i++)
                {
                    frames.GetArrayElementAtIndex(i).objectReferenceValue = row.Frames[i];
                }
                so.FindProperty(slot.SecondsField).floatValue = row.FrameSeconds;

                var renderer = root.GetComponent<SpriteRenderer>();
                SerializedProperty rendererField = so.FindProperty("_renderer");
                if (rendererField != null && renderer != null)
                {
                    rendererField.objectReferenceValue = renderer;
                }
                so.ApplyModifiedPropertiesWithoutUndo();

                if (slot.Still != StillFrame.None && renderer != null)
                {
                    renderer.sprite = slot.Still == StillFrame.First ? row.Frames[0] : row.Frames[row.Frames.Length - 1];
                    renderer.color = Color.white;
                }

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }

        // ---- Report ---------------------------------------------------------------------------

        // Loads every prefab again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            foreach (Look look in Enemies)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(EnemyPrefabFolder, look.Prefab));
                if (prefab == null)
                {
                    Debug.LogError($"{look.Prefab} is missing after the build.");
                    continue;
                }

                var text = new StringBuilder($"{look.Prefab}: ");
                Transform body = prefab.transform.Find(BodyName);
                var renderer = body != null ? body.GetComponent<SpriteRenderer>() : null;
                text.Append(body != null ? $"body child {body.GetSiblingIndex()}, scale {body.localScale.x:0.###}" : "NO BODY");
                text.Append(renderer != null ? $", sprite {NameOf(renderer.sprite)}, order {renderer.sortingOrder}, colour {renderer.color}" : ", NO BODY SPRITE");
                AppendFlipbook(text, body != null ? body.GetComponent<SpriteFlipbook>() : null);
                var face = prefab.GetComponent<FaceTravel>();
                text.Append(face != null ? $"; faces travel, body {NameOf(new SerializedObject(face).FindProperty("_body").objectReferenceValue)}" : "; NO FACE TRAVEL");
                var enemy = prefab.GetComponent<BasicEnemy>();
                text.Append(enemy != null && !new SerializedObject(enemy).FindProperty("_flipSpriteFacing").boolValue ? "; flip off" : "; FLIP STILL ON");
                text.Append(prefab.GetComponent<SpriteRenderer>() == null ? "; no root sprite" : "; OLD ROOT SPRITE STILL THERE");
                text.Append($"; root scale {prefab.transform.localScale.x}");
                Debug.Log(text.ToString());
            }

            foreach (Look look in Towers)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, look.Prefab));
                if (prefab == null)
                {
                    Debug.LogError($"{look.Prefab} is missing after the build.");
                    continue;
                }

                var renderer = prefab.GetComponent<SpriteRenderer>();
                var text = new StringBuilder($"{look.Prefab}: ");
                text.Append(renderer != null ? $"sprite {NameOf(renderer.sprite)}, colour {renderer.color}, order {renderer.sortingOrder}" : "NO SPRITE");
                AppendFlipbook(text, prefab.GetComponent<SpriteFlipbook>());
                text.Append($"; root scale {prefab.transform.localScale.x}");
                Debug.Log(text.ToString());
            }

            foreach (ShotLook look in Shots)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, look.Prefab));
                var effect = look.Effect != null ? AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, look.Effect)) : null;
                if (prefab == null || (look.Effect != null && effect == null))
                {
                    Debug.LogError($"{look.Prefab} or {look.Effect} is missing after the build.");
                    continue;
                }

                var renderer = prefab.GetComponent<SpriteRenderer>();
                var text = new StringBuilder($"{look.Prefab}: ");
                text.Append(renderer != null ? $"sprite {NameOf(renderer.sprite)}, colour {renderer.color}, order {renderer.sortingOrder}" : "NO SPRITE");
                AppendFlipbook(text, prefab.GetComponent<SpriteFlipbook>());
                var face = prefab.GetComponent<FaceTravel>();
                text.Append(face != null ? $"; faces flight, turns {NameOf(new SerializedObject(face).FindProperty("_body").objectReferenceValue)}" : "; does not turn");
                MonoBehaviour owner = prefab.GetComponent<Projectile>();
                if (owner == null)
                {
                    owner = prefab.GetComponent<Mine>();
                }
                string effectField = look.EffectField ?? "_hitEffect";
                SerializedProperty field = owner != null ? new SerializedObject(owner).FindProperty(effectField) : null;
                text.Append($"; {effectField} {(field != null ? NameOf(field.objectReferenceValue) : "MISSING")}");
                text.Append($"; root scale {prefab.transform.localScale.x}");
                Debug.Log(text.ToString());

                if (effect == null)
                {
                    continue;
                }

                var effectRenderer = effect.GetComponent<SpriteRenderer>();
                var oneShot = effect.GetComponent<SpriteOneShot>();
                var effectText = new StringBuilder($"{look.Effect}: ");
                effectText.Append(effectRenderer != null ? $"sprite {NameOf(effectRenderer.sprite)}, order {effectRenderer.sortingOrder}" : "NO SPRITE");
                if (oneShot != null)
                {
                    var so = new SerializedObject(oneShot);
                    effectText.Append($"; plays {so.FindProperty("_frames").arraySize} frames once at {so.FindProperty("_frameSeconds").floatValue:0.###} s");
                }
                else
                {
                    effectText.Append("; NO ONE-SHOT");
                }
                effectText.Append($"; scale {effect.transform.localScale.x}");
                Debug.Log(effectText.ToString());
            }

            foreach (RowSlot slot in Extras)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(TowerPrefabFolder, slot.Prefab));
                Component component = prefab != null ? prefab.GetComponent(slot.Component) : null;
                if (component == null)
                {
                    Debug.LogError($"{slot.Prefab} or its {slot.Component} is missing after the build.");
                    continue;
                }

                var so = new SerializedObject(component);
                SerializedProperty frames = so.FindProperty(slot.FramesField);
                Object first = frames.arraySize > 0 ? frames.GetArrayElementAtIndex(0).objectReferenceValue : null;
                var renderer = prefab.GetComponent<SpriteRenderer>();
                Debug.Log($"{slot.Prefab} {slot.Component}.{slot.FramesField}: {frames.arraySize} frames from {NameOf(first)} at {so.FindProperty(slot.SecondsField).floatValue:0.###} s"
                    + (slot.Still != StillFrame.None && renderer != null ? $"; shows {NameOf(renderer.sprite)}, scale {prefab.transform.localScale.x}, order {renderer.sortingOrder}" : ""));
            }
        }

        private static void AppendFlipbook(StringBuilder text, SpriteFlipbook flipbook)
        {
            if (flipbook == null)
            {
                text.Append("; NO FLIPBOOK");
                return;
            }

            var so = new SerializedObject(flipbook);
            SerializedProperty frames = so.FindProperty("_frames");
            int set = 0;
            for (int i = 0; i < frames.arraySize; i++)
            {
                if (frames.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    set++;
                }
            }
            text.Append($"; flipbook {set}/{frames.arraySize} frames at {so.FindProperty("_frameSeconds").floatValue:0.###} s, renderer {NameOf(so.FindProperty("_renderer").objectReferenceValue)}");
        }

        private static string NameOf(Object asset) => asset != null ? asset.name : "none";
    }
}
