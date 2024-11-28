using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TowerUpgrading : MonoBehaviour
{
    [Header("Upgrade Settings")]
    public float healthIncrease = 20f;
    public int shieldIncrease = 10;
    public int damageIncrease = 5;
    public float upgradeCost = 50f;

    [Header("UI Elements")]
    public Button upgradeButton;       
    public TMP_Text feedbackText;      
    public TMP_Text balanceText;       

    private void Start()
    {
        upgradeButton.onClick.AddListener(UpgradeTowers);

        UpdateBalanceUI();
    }

    private void UpgradeTowers()
    {
        if (GameStatsManager.Instance.totalGold >= upgradeCost)
        {
            GameStatsManager.Instance.SpendGold(upgradeCost);

            PlayerPrefs.SetFloat("TotalGold", GameStatsManager.Instance.totalGold);
            PlayerPrefs.Save();

            UpdateBalanceUI();

            CharacterStats[] soldiers = FindObjectsOfType<CharacterStats>();
            foreach (var soldier in soldiers)
            {
                if (soldier.characterType == CharacterStats.CharacterType.Soldier)
                {
                    soldier.maxHealth += healthIncrease;
                    soldier.shield += shieldIncrease;
                    soldier.hasShield = soldier.shield > 0;
                }
            }

            SoldierAI[] soldierAIs = FindObjectsOfType<SoldierAI>();
            foreach (var soldierAI in soldierAIs)
            {
                soldierAI.UpgradeDamage(damageIncrease);
            }

            StartCoroutine(ShowFeedback("Towers upgraded! Soldiers are stronger."));
            Debug.Log("Towers upgraded successfully.");
        }
        else
        {
            StartCoroutine(ShowFeedback("Not enough gold to upgrade towers!"));
            Debug.Log("Upgrade failed: Not enough gold.");
        }
    }

    private void UpdateBalanceUI()
    {
        balanceText.text = "Gold: " + GameStatsManager.Instance.totalGold.ToString("F2");
    }

    private IEnumerator ShowFeedback(string message)
    {
        feedbackText.text = message;
        yield return new WaitForSeconds(3f); 
        feedbackText.text = string.Empty;    
    }
}
