using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownUI : MonoBehaviour
{
    public SpellManager spellManager;
    public TextMeshProUGUI cooldownText;
    public Button spellActivateButton;

    void Update()
    {
        if (spellManager != null)
        {
            float remainingTime = Mathf.Max(0, spellManager.cooldownTimer);

            if (remainingTime > 0)
            {
                cooldownText.text = $"{remainingTime:F1}s";
                cooldownText.gameObject.SetActive(true);
            }
            else
            {
                cooldownText.text = "";
                cooldownText.gameObject.SetActive(false);
            }
            spellActivateButton.interactable = remainingTime <= 0;
        }
    }
}
