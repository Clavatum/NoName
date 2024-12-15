using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthCheck : MonoBehaviour
{
    public TMP_Text logTxt;
    public TMP_Text statusTxt; // Status göstermek için yeni text
    private bool isCheckingConnection = false;
    private bool isConnectionLost = false;
    private float countdownTime = 5f; // Geri sayým süresi

    private void Start()
    {
        statusTxt.text = "Status: Online"; // Baþlangýç durumu
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
            yield return new WaitForSeconds(1f); // Her saniye baðlantýyý kontrol eder
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
                    OnConnectionLost(); // Baðlantý ilk kez kaybedildiðinde baþlat
                }
            }
            else
            {
                if (isConnectionLost)
                {
                    OnConnectionRestored(); // Baðlantý tekrar geldiðinde
                }
            }
        }

        isCheckingConnection = false;
    }

    void OnConnectionLost()
    {
        isConnectionLost = true;
        Time.timeScale = 0; // Oyunu duraklatýr
        statusTxt.text = "Status: Connection Lost";
        countdownTime = 5f; // Geri sayýmý baþlat
        StartCoroutine(CountdownToReturn());
    }

    void OnConnectionRestored()
    {
        isConnectionLost = false;
        Time.timeScale = 1; // Oyunu devam ettirir
        statusTxt.text = "Status: Online";
        logTxt.gameObject.SetActive(false); // Hata mesajýný gizler
        StopCoroutine(CountdownToReturn()); // Geri sayýmý durdurur
    }

    IEnumerator CountdownToReturn()
    {
        logTxt.gameObject.SetActive(true);

        while (countdownTime > 0)
        {
            logTxt.text = $"Internet connection lost! Returning in {countdownTime:F0} seconds...";
            yield return new WaitForSecondsRealtime(1f); // Her saniye günceller
            countdownTime--;

            // Baðlantý geri geldiyse geri sayýmý durdurur
            if (!isConnectionLost)
            {
                yield break;
            }
        }

        // Geri sayým bittiðinde giriþ ekranýna döner
        if (isConnectionLost)
        {
            ReturnToSignIn();
        }
    }

    void ReturnToSignIn()
    {
        SceneManager.LoadScene(0); // Giriþ ekranýna döner
        logTxt.gameObject.SetActive(false);
    }
}
