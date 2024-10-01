using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthCheck : MonoBehaviour
{
    public TMP_Text logTxt;

    private void Update()
    {
        CheckInternetConnection();
    }

    void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            logTxt.gameObject.SetActive(true);
            Debug.Log("No Internet Connection");
            logTxt.text = "Internet connection lost!";
            ShowErrorAndReturnToSignIn(); 
        }
    }

    void LoadGameSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void ShowErrorAndReturnToSignIn()
    {
        logTxt.text += "\nReturning to sign-in screen...";
        Invoke("ReturnToSignIn", 2f); 
    }

    void ReturnToSignIn()
    {
        LoadGameSceneByIndex(0);
        logTxt.gameObject.SetActive(false);
    }
}
