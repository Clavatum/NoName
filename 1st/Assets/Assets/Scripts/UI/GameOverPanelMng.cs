using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;


public class GameOverPanelMng : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text escapeCounterText;
    public Button playAgainButton;
    public Button backToMenuButton;

    private Animator animator;
    public bool isGameOver;
    public bool hasShownGameOver = false;

    public HealthPotion healthPotion;

    void Awake()
    {
        gameOverPanel.SetActive(false);

        playAgainButton.onClick.AddListener(PlayAgain);
        backToMenuButton.onClick.AddListener(BackToMenu);

        animator = gameOverPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isGameOver && !hasShownGameOver)
        {
            SaveGameDataBeforeGameOver();
            ShowGameOverPanel();
            healthPotion.shopPanel.SetActive(false);
            hasShownGameOver = true;
        }
    }

    private async void SaveGameDataBeforeGameOver()
    {
        await CloudSaveManager.SaveToCloud(CloudSaveManager.CollectDataForSave());
        Debug.Log("Game data saved to the cloud before showing game over panel.");
    }

    void PlayAgain()
    {
        PlayerPrefs.SetFloat("TotalGold", GameStatsManager.Instance.totalGold);
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("GameScene");
        escapeCounterText.gameObject.SetActive(true);
        PatrolEndTrigger.escapedEnemiesCount = 0;
    }

    void BackToMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("Menu");
    }

    async void ShowGameOverPanel()
    {
        await Task.Delay(400);
        AudioManager.Instance.PlayGameOverSound();
        gameOverPanel.SetActive(true);
        animator.SetTrigger("Show");
        StartCoroutine(StopGameAfterAnimation());
    }

    private IEnumerator StopGameAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Time.timeScale = 0f;
    }
}
