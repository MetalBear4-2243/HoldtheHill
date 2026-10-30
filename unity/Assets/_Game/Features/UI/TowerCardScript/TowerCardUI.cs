//Adam Montgomery
//Gemini generated code
//10.22.2026

using UnityEngine;
using UnityEngine.UI;
using HoldTheHill.Features.Towers;
using TMPro;

public class TowerCardUI : MonoBehaviour
{
    public TowerData towerData;
    public TowerPlacer towerPlacer;
    public Image iconDisplay;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI costDisplay;

    // 1. The shop script calls this exactly when it spawns the card
    public void SetupCard(TowerData data)
    {
        towerData = data;
        
        // 2. Overwrite the default UI with the real data
        if (iconDisplay != null) iconDisplay.sprite = data.towerIcon;
        if (nameDisplay != null) nameDisplay.text = data.towerName;
        if (costDisplay != null) costDisplay.text = data.honeydewCost.ToString();
    }

    void Start()
    {
        // 2. Prefabs can't hold scene references, so we find the Placer dynamically
        if (towerPlacer == null)
        {
            towerPlacer = FindFirstObjectByType<TowerPlacer>(); 
        }

        // 3. Set up the click detection
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnCardClicked);
        }
    }

    public void OnCardClicked()
    {
        if (towerPlacer != null && towerData != null)
        {
            towerPlacer.Select(towerData);
        }
    }
}