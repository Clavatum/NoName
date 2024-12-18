using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private float potionCost = 50f; // Cost of the potion
    [SerializeField] private float healPercentage = 50f; // Percentage to heal
    [SerializeField] private TextMeshProUGUI feedbackText; // Feedback text (optional)
    [SerializeField] private Button useHealthPotionButton;
    [SerializeField] private Button openShopPanelButton;
    [SerializeField] private GameObject shopPanel;

    private bool isShopOpen = false;
    private GameStatsManager statsManager;
    [SerializeField] private HealthSystem playerHealth;

    void Awake()
    {
        useHealthPotionButton.onClick.AddListener(UsePotion);
        openShopPanelButton.onClick.AddListener(OpenShopPanel);
    }

    private void Start()
    {
        statsManager = GameStatsManager.Instance;
    }

    public void OpenShopPanel()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);
    }

    public void UsePotion()
    {
        // Check if player can afford the potion
        if (statsManager.totalGold < potionCost)
        {
            DisplayFeedback("Not enough gold!");
            return;
        }

        // Check if player's health is already full
        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            DisplayFeedback("Health is already full!");
            return;
        }

        // Deduct gold
        statsManager.SpendGold(potionCost);

        // Calculate healing amount
        float maxHeal = playerHealth.maxHealth * (healPercentage / 100f);
        float healAmount = Mathf.Min(maxHeal, playerHealth.maxHealth - playerHealth.currentHealth);

        // Heal the player
        playerHealth.currentHealth += healAmount;
        playerHealth.UpdateHealthBar();

        // Provide feedback
        DisplayFeedback($"Healed for {healAmount} HP!");

        Debug.Log($"Potion used: Healed {healAmount} HP, remaining gold: {statsManager.totalGold}");
    }

    private void DisplayFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            Invoke(nameof(ClearFeedback), 2f); // Clear message after 2 seconds
        }
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
        }
    }
}
