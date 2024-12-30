using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class YouWinPanelMng : MonoBehaviour
{
    public GameObject youWinPanel;

    private GameStatsManager gameStatsManager;

    private Animator animator;

    public static bool gameWon;
    private bool hasShownYouWin = false;

    void Awake()
    {
        youWinPanel.SetActive(false);

        gameStatsManager = GameStatsManager.Instance;

        animator = youWinPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameWon && !hasShownYouWin)
        {
            SaveGameDataBeforeYouWin();
            ShowYouWinPanel();
        }
    }

    private void SaveGameDataBeforeYouWin()
    {
        gameStatsManager.CompleteGame();
        Debug.Log("Game data saved before 'You Win' screen.");
    }

    async void ShowYouWinPanel()
    {
        await Task.Delay(750);
        AudioManager.Instance.PlayGameWinSound();
        youWinPanel.SetActive(true);
        animator.SetTrigger("Show");
        hasShownYouWin = true;

        StartCoroutine(StopGameAfterAnimation());
    }

    private IEnumerator StopGameAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        StartCoroutine(LoadGameReviewScene());
    }

    private IEnumerator LoadGameReviewScene()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("GameReview");
    }
}
