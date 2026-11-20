//Adam Montgomery
//Gemini generated code
//10.22.2026

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HoldTheHill.Features.Towers;
using TMPro;

// Add the IPointerEnterHandler and IPointerExitHandler interfaces here
public class TowerCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TowerData towerData;
    public TowerPlacer towerPlacer;
    public Image iconDisplay;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI costDisplay;

    [Header("Selection")]
    [Tooltip("The card's background tint while its tower is picked (the ghost is out for it).")]
    public Color selectedTint = new Color(1f, 0.82f, 0.3f, 1f);

    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private Image _background;
    private Color _normalTint = Color.white;
    private bool _showingSelected;

    void Awake()
    {
        // The card's own background image, on the same object as this script.
        _background = GetComponent<Image>();
        if (_background != null)
        {
            _normalTint = _background.color;
        }

        // Ensure the tooltip is hidden when the card first spawns
        if (tooltipPanel != null) 
        {
            tooltipPanel.SetActive(false);
        }
    }

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
            towerPlacer = FindAnyObjectByType<TowerPlacer>();
        }

        // 3. Set up the click detection
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnCardClicked);
        }
    }

    // The card is highlighted while its tower is the one in hand: picked from this card, by its
    // number key, or still in hand after a build. A placer read per card per frame, nothing more.
    void Update()
    {
        bool selected = IsPicked();
        if (selected == _showingSelected)
        {
            return;
        }

        _showingSelected = selected;
        if (_background != null)
        {
            _background.color = selected ? selectedTint : _normalTint;
        }
    }

    /// <summary>True while the placer holds this card's tower, ready to build.</summary>
    public bool IsPicked()
    {
        return towerPlacer != null && towerData != null
            && towerPlacer.Selected.HasValue && towerPlacer.Selected.Value.Data == towerData;
    }

    // Picks this card's tower; clicking the card again puts it away. Neither works while paused.
    public void OnCardClicked()
    {
        if (towerPlacer == null || towerData == null || !towerPlacer.AcceptsInput)
        {
            return;
        }

        if (IsPicked())
        {
            towerPlacer.Select(-1);
        }
        else
        {
            towerPlacer.Select(towerData);
        }
    }

    // --- HOVER FUNCTIONALITY ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null && tooltipText != null && towerData != null)
        {
            // Note: You will need to add a "public string description;" variable 
            // to your TowerData.cs script if it doesn't have one already!
            tooltipText.text = towerData.towerDescription; 
            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}