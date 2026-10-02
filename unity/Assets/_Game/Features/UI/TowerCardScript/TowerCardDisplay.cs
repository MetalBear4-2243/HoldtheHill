/// Adam Montgomery
/// 10/2/2026
/// This code has been generated entirely using Google Gemini

using UnityEngine;
using UnityEngine.UI; // Required for the Image component
using TMPro; // Required for TextMeshPro text components

public class TowerCardDisplay : MonoBehaviour
{
    [Header("UI Component References")]
    public Image towerIcon;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI costText;

    /// <summary>
    /// Call this method from your game manager or shop controller 
    /// to populate the card with specific tower data.
    /// </summary>
    public void SetupCard(string name, int cost, Sprite newIcon)
    {
        // Update text fields
        towerNameText.text = name;
        costText.text = cost.ToString();
        
        // Swap out the placeholder ant for the real sprite
        towerIcon.sprite = newIcon;
    }
}
