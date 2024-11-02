using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanelMng : MonoBehaviour
{
    public GameObject gameOverPanel;      
    public Button playAgainButton;        
    public Button backToMenuButton;       

    private Animator animator;
    public bool isGameOver;
    private bool hasShownGameOver = false;  

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
            ShowGameOverPanel();  
        }
    }

    void PlayAgain()
    {
        Time.timeScale = 1f;       
        isGameOver = false;
        SceneManager.LoadScene("GameScene");
    }

    void BackToMenu()
    {
        Time.timeScale = 1f;       
        isGameOver = false;
        SceneManager.LoadScene("Menu");
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);    
        animator.SetTrigger("Show");      
        hasShownGameOver = true;          

        StartCoroutine(StopGameAfterAnimation());
    }

    private IEnumerator StopGameAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Time.timeScale = 0f;              
    }
}
