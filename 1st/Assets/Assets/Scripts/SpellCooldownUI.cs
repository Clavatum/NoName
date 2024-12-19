using TMPro;
using UnityEngine;

public class SpellCooldownUI : MonoBehaviour
{
    public SpellManager spellManager;
    public TextMeshProUGUI cooldownText;

    void Update()
    {
        if (spellManager != null)
        {
            float remainingTime = Mathf.Max(0, spellManager.spellCooldown - spellManager.cooldownTimer);
            cooldownText.text = remainingTime > 0 ? $"Spell Cooldown: {remainingTime:F1}s" : "Spell Ready!";
        }
    }
}
