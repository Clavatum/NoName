using UnityEngine;
using TMPro;

public class PatrolEndTrigger : MonoBehaviour
{
    [Header("Escape Counter Settings")]
    [SerializeField] private TMP_Text escapeCounterText;
    [SerializeField] private int maxAllowedEscapes = 10;
    public static int escapedEnemiesCount;
    private GameOverPanelMng gameOverManager;

    private void Start()
    {
        gameOverManager = FindObjectOfType<GameOverPanelMng>();
        UpdateEscapeCounterUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        escapedEnemiesCount++;
        UpdateEscapeCounterUI();

        if (escapedEnemiesCount >= maxAllowedEscapes)
        {
            TriggerGameOver();
        }

        Destroy(other.gameObject);
    }

    private void UpdateEscapeCounterUI()
    {
        if (escapeCounterText != null)
        {
            escapeCounterText.text = $"{escapedEnemiesCount}/{maxAllowedEscapes}";
        }
    }

    private void TriggerGameOver()
    {
        if (gameOverManager != null)
        {
            gameOverManager.isGameOver = true;
            GameStatsManager.Instance.CompleteGame();
        }
    }
}
