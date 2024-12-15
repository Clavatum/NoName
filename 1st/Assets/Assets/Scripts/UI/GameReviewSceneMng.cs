using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  

public class GameReviewSceneMng : MonoBehaviour
{
    public Button playAgainButton;
    public Button backToMenuButton;

    public TMP_Text completionTimeText;
    public TMP_Text totalKillsText;
    public TMP_Text goldSpentText;
    public TMP_Text goldEarnedText;
    public TMP_Text totalGoldText;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    void Start()
    {
        completionTimeText.text = "Completion Time: " + FormatTime(GameStatsManager.Instance.completionTime);
        totalKillsText.text = "Total Kills: " + GameStatsManager.Instance.totalKillsInGame;
        goldSpentText.text = "Gold Spent: " + GameStatsManager.Instance.goldSpent;
        goldEarnedText.text = "Gold Earned: " + GameStatsManager.Instance.goldEarned;
        totalGoldText.text = "Total Gold: " + GameStatsManager.Instance.totalGold;
    }

    void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
