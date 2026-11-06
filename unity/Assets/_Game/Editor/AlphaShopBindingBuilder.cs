using System.Linq;
using HoldTheHill.Features.Towers;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Fits Adam's TowerList (his #45) into the run, in the <c>UI</c> scene only: a
    /// <see cref="ShopPlacerBinding"/> as an added component on the prefab instance, so his cards
    /// follow Restart and the next level; the instance's tower list set to the six towers in
    /// number-key order; and a <see cref="HoneydewLabel"/> on his balance box, so it shows the
    /// wallet instead of a fixed 150. His prefab and scripts are not touched; all are scene overrides.
    /// </summary>
    /// <remarks>Safe to run again: components are added only when missing and the list is set, not appended.</remarks>
    internal static class AlphaShopBindingBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";

        // The text in Adam's BalanceBox (TowerList.prefab).
        private const string BalanceTextName = "BalanceAmount";

        // Number-key order, the same as the placer's offers: 1 is the Ant, 6 the Carpenter Ant.
        private static readonly string[] TowerKeyOrder = { "Ant", "BulletAnt", "CrazyAnt", "FireAnt", "SapAnt", "CarpenterAnt" };

        [MenuItem("Tools/Hold the Hill/Alpha/Bind Shop List to the Level")]
        private static void Build()
        {
            // From the menu, the open scenes are about to be replaced: offer to save them first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerShopManager shop = AlphaRunFrameBuilder.FindInScene<TowerShopManager>(ui);
            if (shop == null)
            {
                Debug.LogError("The UI scene has no TowerList (Adam's shop) to bind.");
                return;
            }

            EnsureBinding(shop);
            SetTowerList(shop);
            EnsureBalanceLabel(shop);
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

        private static void EnsureBalanceLabel(TowerShopManager shop)
        {
            TMP_Text balance = FindBalanceText(shop);
            if (balance == null)
            {
                Debug.LogError($"Adam's shop list has no {BalanceTextName} text to bind to the wallet.");
                return;
            }

            HoneydewLabel label = balance.GetComponent<HoneydewLabel>();
            if (label == null)
            {
                label = balance.gameObject.AddComponent<HoneydewLabel>();
            }

            var so = new SerializedObject(label);
            so.FindProperty("_amountOnly").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static TMP_Text FindBalanceText(TowerShopManager shop)
        {
            return shop.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(t => t.name == BalanceTextName);
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
            TMP_Text balance = FindBalanceText(shop);
            bool balanceBound = balance != null && balance.GetComponent<HoneydewLabel>() != null;
            string towers = string.Join(", ", shop.availableTowers.Select(t => t != null ? t.name : "none"));
            Debug.Log($"Shop list: binding = {(bound ? "yes" : "MISSING")}, balance = {(balanceBound ? "wallet" : "MISSING")}, towers = {towers}");
        }
    }
}
