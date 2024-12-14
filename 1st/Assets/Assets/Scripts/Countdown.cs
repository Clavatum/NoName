using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private GameOverPanelMng gameOverPanelMng;
    private float countDownTimer = 3f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CountDown());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            countDownTimer = 3f;
            countDownText.text = "";
            StopAllCoroutines();
        }
    }

    private IEnumerator CountDown()
    {
        if (countDownText != null)
        {
            while (countDownTimer != 0f)
            {
                countDownText.text = "Return to game in " + countDownTimer.ToString("F0") + " second(s)!";
                yield return new WaitForSecondsRealtime(1f);
                countDownTimer--;
            }
            gameOverPanelMng.isGameOver = true;
            countDownTimer = 3f;
            countDownText.text = "";
        }
    }
}
