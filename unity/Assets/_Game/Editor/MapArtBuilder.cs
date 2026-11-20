using HoldTheHill.Features.Enemies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Map art (2026-11-20, Nilly's picks for the Beta): one baked picture per level, with the ground,
    /// the road, props, the spawn burrow and the Queen's mound drawn in. It sits as a "Map Art" sprite in
    /// the level's <c>_Terrain</c> scene; the level's <c>Enemy Path</c> stops laying its plain road.
    /// </summary>
    /// <remarks>
    /// The pictures come from <c>B:\Projects\GameDev\Source\2D\hold-the-hill-map-art\map_art.py</c>
    /// (Cartridge 64, 64 px per world unit), which draws the road from the waypoints in the
    /// <c>_MapArt.json</c> beside each picture. If a level's path has been moved since, this builder
    /// warns and leaves the plain road on, so the road enemies walk is always visible; redraw the picture
    /// with the new waypoints and run this again. Safe to run again.
    /// To take the art off: delete the "Map Art" object from the terrain scenes and tick Draw Road on
    /// the Enemy Paths.
    /// </remarks>
    internal static class MapArtBuilder
    {
        private const string ArtFolder = "Assets/_Game/Features/Levels/Art";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";
        private const string ObjectName = "Map Art";
        private const string RoadChildName = "Road";
        private const int SortingOrder = -100;

        private static readonly string[] Levels = { "L00_TutorialHill", "L01_Meadow" };

        [System.Serializable]
        private class ArtInfo
        {
            public string id;
            public float pixelsPerUnit;
            public float[] worldRect;
            public float[] waypointsFlat;
        }

        [MenuItem("Tools/Hold the Hill/Build Map Art (Tutorial Hill, Meadow)")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            foreach (string id in Levels)
            {
                BuildLevel(id);
            }
        }

        private static void BuildLevel(string id)
        {
            string pngPath = $"{ArtFolder}/{id}_MapArt.png";
            string jsonPath = $"{ArtFolder}/{id}_MapArt.json";
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
            if (json == null)
            {
                Debug.LogError($"Map art: {jsonPath} is missing.");
                return;
            }

            ArtInfo info = JsonUtility.FromJson<ArtInfo>(json.text);
            Sprite sprite = ImportSprite(pngPath, info.pixelsPerUnit);
            if (sprite == null)
            {
                Debug.LogError($"Map art: {pngPath} is missing or is not a sprite.");
                return;
            }

            string folder = $"{LevelsFolder}/{id}";
            bool matches = SetRoad($"{folder}/{id}_Gameplay.unity", info.waypointsFlat, id);
            PlaceArt($"{folder}/{id}_Terrain.unity", sprite, info.worldRect);
            Debug.Log(matches
                ? $"Map art: {id} done (picture in the terrain scene, plain road off)."
                : $"Map art: {id} picture placed, but the plain road stays on because the path no longer matches it.");
        }

        private static Sprite ImportSprite(string path, float pixelsPerUnit)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return null;
            }

            // Pixel art (UNITY_RULES 24): point filter, no compression, no mipmaps.
            bool changed = importer.textureType != TextureImporterType.Sprite
                || importer.spriteImportMode != SpriteImportMode.Single
                || !Mathf.Approximately(importer.spritePixelsPerUnit, pixelsPerUnit)
                || importer.filterMode != FilterMode.Point
                || importer.textureCompression != TextureImporterCompression.Uncompressed
                || importer.mipmapEnabled
                || importer.maxTextureSize < 4096;
            if (changed)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 4096;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // Turns the plain road off when the scene's waypoints are the ones the picture was drawn from.
        private static bool SetRoad(string scenePath, float[] flat, string id)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            EnemyPath path = AlphaRunFrameBuilder.FindInScene<EnemyPath>(scene);
            if (path == null)
            {
                Debug.LogError($"Map art: no Enemy Path in {scenePath}.");
                return false;
            }

            bool matches = WaypointsMatch(path.transform, flat, id);
            var serialized = new SerializedObject(path);
            SerializedProperty drawRoad = serialized.FindProperty("_drawRoad");
            if (drawRoad.boolValue == matches)
            {
                drawRoad.boolValue = !matches;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            return matches;
        }

        private static bool WaypointsMatch(Transform path, float[] flat, string id)
        {
            int index = 0;
            foreach (Transform child in path)
            {
                if (child.name == RoadChildName)
                {
                    continue;
                }

                if (index + 1 >= flat.Length
                    || Mathf.Abs(child.position.x - flat[index]) > 0.01f
                    || Mathf.Abs(child.position.y - flat[index + 1]) > 0.01f)
                {
                    Debug.LogWarning($"Map art: {id} waypoint {index / 2} ({child.position.x}, {child.position.y}) is not where the picture's road is. Redraw the map art.");
                    return false;
                }

                index += 2;
            }

            if (index != flat.Length)
            {
                Debug.LogWarning($"Map art: {id} has {index / 2} waypoints but the picture was drawn for {flat.Length / 2}. Redraw the map art.");
                return false;
            }

            return true;
        }

        private static void PlaceArt(string scenePath, Sprite sprite, float[] worldRect)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            GameObject art = null;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == ObjectName)
                {
                    art = root;
                }
            }

            if (art == null)
            {
                art = new GameObject(ObjectName);
                SceneManager.MoveGameObjectToScene(art, scene);
            }

            var renderer = art.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = art.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = sprite;
            renderer.sortingOrder = SortingOrder;
            // The sprite's pivot is its centre; the picture covers worldRect (x, y, width, height).
            art.transform.position = new Vector3(worldRect[0] + worldRect[2] * 0.5f, worldRect[1] + worldRect[3] * 0.5f, 0f);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
