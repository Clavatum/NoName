using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthCheck : MonoBehaviour
{
    public TMP_Text logTxt;
    public TMP_Text statusTxt; // Status g�stermek i�in yeni text
    private bool isCheckingConnection = false;
    private bool isConnectionLost = false;
    private float countdownTime = 5f; // Geri say�m s�resi

    private void Start()
    {
        statusTxt.text = "Status: Online"; // Ba�lang�� durumu
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
            yield return new WaitForSeconds(1f); // Her saniye ba�lant�y� kontrol eder
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
                    OnConnectionLost(); // Ba�lant� ilk kez kaybedildi�inde ba�lat
                }
            }
            else
            {
                if (isConnectionLost)
                {
                    OnConnectionRestored(); // Ba�lant� tekrar geldi�inde
                }
            }
        }

        isCheckingConnection = false;
    }

    void OnConnectionLost()
    {
        isConnectionLost = true;
        Time.timeScale = 0; // Oyunu duraklat�r
        statusTxt.text = "Status: Connection Lost";
        countdownTime = 5f; // Geri say�m� ba�lat
        StartCoroutine(CountdownToReturn());
    }

    void OnConnectionRestored()
    {
        isConnectionLost = false;
        Time.timeScale = 1; // Oyunu devam ettirir
        statusTxt.text = "Status: Online";
        logTxt.gameObject.SetActive(false); // Hata mesaj�n� gizler
        StopCoroutine(CountdownToReturn()); // Geri say�m� durdurur
    }

    IEnumerator CountdownToReturn()
    {
        logTxt.gameObject.SetActive(true);

        while (countdownTime > 0)
        {
            logTxt.text = $"Internet connection lost! Returning in {countdownTime:F0} seconds...";
            yield return new WaitForSecondsRealtime(1f); // Her saniye g�nceller
            countdownTime--;

            // Ba�lant� geri geldiyse geri say�m� durdurur
            if (!isConnectionLost)
            {
                yield break;
            }
        }

        // Geri say�m bitti�inde giri� ekran�na d�ner
        if (isConnectionLost)
        {
            ReturnToSignIn();
        }
    }

    void ReturnToSignIn()
    {
        SceneManager.LoadScene(0); // Giri� ekran�na d�ner
        logTxt.gameObject.SetActive(false);
    }
}
