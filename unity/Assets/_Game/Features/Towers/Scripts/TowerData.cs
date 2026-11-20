//Adam Montgomery
//Gemini generated code
//10.9.2026

using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// What the shop shows for one tower: its name, price and icon. The placer's offers and the
    /// shop cards both point at these assets (in Features/Towers/Data).
    /// </summary>
    [CreateAssetMenu(fileName = "New Tower", menuName = "Hold the Hill/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("Shop Display")]
        public string towerName;
        public int honeydewCost;
        public Sprite towerIcon;

        public string towerDescription;
    }
}
