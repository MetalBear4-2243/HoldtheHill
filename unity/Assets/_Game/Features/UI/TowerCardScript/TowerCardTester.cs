/// Adam Montgomery
/// 10/2/2026
/// Generated entirely using Google Gemini

using UnityEngine;

public class TowerCardTester : MonoBehaviour
{
    [Header("UI Reference")]
    public TowerCardDisplay targetCard;

    [Header("Test Data")]
    public string testName = "Spitter Ant";
    public int testCost = 150;
    public Sprite testSprite; 

    void Start()
    {
        // Check if the card is assigned before trying to update it
        if (targetCard != null)
        {
            // Push our test data into the card display script
            targetCard.SetupCard(testName, testCost, testSprite);
        }
        else
        {
            Debug.LogWarning("No card assigned to the TowerCardTester!");
        }
    }
}