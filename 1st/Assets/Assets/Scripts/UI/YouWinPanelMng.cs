using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class YouWinPanelMng : MonoBehaviour
{
    public GameObject youWinPanel;

    private Animator animator;

    public bool gameWon;
    private bool hasShownYouWin = false;

    void Awake()
    {
        youWinPanel.SetActive(false);

        animator = youWinPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameWon && !hasShownYouWin)
        {
            ShowGameOverPanel();
        }
    }

    public void ShowGameOverPanel()
    {
        youWinPanel.SetActive(true);
        animator.SetTrigger("Show");
        hasShownYouWin = true;

        StartCoroutine(StopGameAfterAnimation());
    }

    private IEnumerator StopGameAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        //Time.timeScale = 0f;
        StartCoroutine(LoadGameReviewScene());
    }

    private IEnumerator LoadGameReviewScene() 
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("GameReview"); 
    }
}
