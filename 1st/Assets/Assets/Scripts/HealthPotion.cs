using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private float potionCost = 50f;
    [SerializeField] private float healPercentage = 50f;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Button useHealthPotionButton;
    [SerializeField] private Button openShopPanelButton;
    public GameObject shopPanel;

    public static bool isShopOpen = false;
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
        if (statsManager.totalGold < potionCost)
        {
            DisplayFeedback("Not enough gold!");
            return;
        }

        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            DisplayFeedback("Health is already full!");
            return;
        }

        statsManager.SpendGold(potionCost);

        float maxHeal = playerHealth.maxHealth * (healPercentage / 100f);
        float healAmount = Mathf.Min(maxHeal, playerHealth.maxHealth - playerHealth.currentHealth);

        playerHealth.currentHealth += healAmount;
        playerHealth.UpdateHealthBar();

        DisplayFeedback($"Healed for {healAmount} HP!");

        Debug.Log($"Potion used: Healed {healAmount} HP, remaining gold: {statsManager.totalGold}");
    }

    private void DisplayFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            Invoke(nameof(ClearFeedback), 2f);
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
