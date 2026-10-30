using System.Linq;
using HoldTheHill.Features.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Fits Adam's TowerList (his #45) into the run, in the <c>UI</c> scene only: a
    /// <see cref="ShopPlacerBinding"/> as an added component on the prefab instance, so his cards
    /// follow Restart and the next level, and the instance's tower list set to the six towers in
    /// number-key order. His prefab and scripts are not touched; both are scene overrides.
    /// </summary>
    /// <remarks>Safe to run again: the binding is added only when missing and the list is set, not appended.</remarks>
    internal static class AlphaShopBindingBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";

        // Number-key order, the same as the placer's offers: 1 is the Ant, 6 the Carpenter Ant.
        private static readonly string[] TowerKeyOrder = { "Ant", "BulletAnt", "CrazyAnt", "FireAnt", "SapAnt", "CarpenterAnt" };

        [MenuItem("Tools/Hold the Hill/Alpha/Bind Shop List to the Level")]
        private static void Build()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerShopManager shop = AlphaRunFrameBuilder.FindInScene<TowerShopManager>(ui);
            if (shop == null)
            {
                Debug.LogError("The UI scene has no TowerList (Adam's shop) to bind.");
                return;
            }

            EnsureBinding(shop);
            SetTowerList(shop);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);

            ReportAfterReload();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaShopBindingBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        private static void EnsureBinding(TowerShopManager shop)
        {
            if (shop.GetComponent<ShopPlacerBinding>() != null)
            {
                return;
            }

            // On a prefab instance this is an added-component override; the prefab stays as Adam left it.
            shop.gameObject.AddComponent<ShopPlacerBinding>();
        }

        // Loaded after OpenScene: opening a scene unloads loose ScriptableObjects loaded before it.
        private static void SetTowerList(TowerShopManager shop)
        {
            var so = new SerializedObject(shop);
            SerializedProperty list = so.FindProperty("availableTowers");
            list.arraySize = TowerKeyOrder.Length;
            for (int i = 0; i < TowerKeyOrder.Length; i++)
            {
                string path = $"{TowerDataFolder}/{TowerKeyOrder[i]}.asset";
                var data = AssetDatabase.LoadAssetAtPath<TowerData>(path);
                if (data == null)
                {
                    Debug.LogError($"Tower data is missing at {path}.");
                }
                list.GetArrayElementAtIndex(i).objectReferenceValue = data;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Opens the saved scene again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerShopManager shop = AlphaRunFrameBuilder.FindInScene<TowerShopManager>(ui);
            if (shop == null)
            {
                Debug.LogError("The TowerList is missing from the UI scene.");
                return;
            }

            bool bound = shop.GetComponent<ShopPlacerBinding>() != null;
            string towers = string.Join(", ", shop.availableTowers.Select(t => t != null ? t.name : "none"));
            Debug.Log($"Shop list: binding = {(bound ? "yes" : "MISSING")}, towers = {towers}");
        }
    }
}
