using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthCheck : MonoBehaviour
{
    public TMP_Text logTxt;
    public TMP_Text statusTxt;
    private bool isCheckingConnection = false;
    private bool isConnectionLost = false;
    private float countdownTime = 5f;

    private void Start()
    {
        statusTxt.text = "Status: Online";
        StartCoroutine(CheckInternetConnectionRoutine());
    }

    IEnumerator CheckInternetConnectionRoutine()
    {
        while (true)
        {
            if (!isCheckingConnection)
            {
                StartCoroutine(CheckInternetConnection());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        isCheckingConnection = true;

        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (!isConnectionLost)
                {
                    OnConnectionLost();
                }
            }
            else
            {
                if (isConnectionLost)
                {
                    OnConnectionRestored();
                }
            }
        }

        isCheckingConnection = false;
    }

    void OnConnectionLost()
    {
        isConnectionLost = true;
        Time.timeScale = 0;
        statusTxt.text = "Status: Connection Lost";
        countdownTime = 5f;
        StartCoroutine(CountdownToReturn());
    }

    void OnConnectionRestored()
    {
        isConnectionLost = false;
        Time.timeScale = 1;
        statusTxt.text = "Status: Online";
        logTxt.gameObject.SetActive(false);
        StopCoroutine(CountdownToReturn());
    }

    IEnumerator CountdownToReturn()
    {
        logTxt.gameObject.SetActive(true);

        while (countdownTime > 0)
        {
            logTxt.text = $"Internet connection lost! Returning in {countdownTime:F0} seconds...";
            yield return new WaitForSecondsRealtime(1f);
            countdownTime--;

            if (!isConnectionLost)
            {
                yield break;
            }
        }

        if (isConnectionLost)
        {
            ReturnToSignIn();
        }
    }

    void ReturnToSignIn()
    {
        SceneManager.LoadScene(0);
        logTxt.gameObject.SetActive(false);
    }
}
