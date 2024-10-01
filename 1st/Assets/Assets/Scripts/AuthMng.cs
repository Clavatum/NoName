using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class authManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text logTxt;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    void Update() // this part will be updated when it s added on other scenes.
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No Internet Connection");
            logTxt.text = "Internet connection lost!";
        }
    }

    public async void SignIn()
    {
        string userName = userNameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            await SignInWithUsernamePasswordAsync(userName, password);
        }
        else
        {
            logTxt.text = "Username or password cannot be empty!";
        }
    }

    public async void SignUp()
    {
        string userName = userNameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            await SignUpWithUsernamePasswordAsync(userName, password);
        }
        else
        {
            logTxt.text = "Username or password cannot be empty!";
        }
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
            logTxt.text = "SignUp is successful.";
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "SignUp failed: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "Request failed: " + ex.Message;
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
            logTxt.text = "SignIn is successful.";
            LoadGameSceneByIndex(1);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "SignIn failed: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "Request failed: " + ex.Message;
        }
    }

    void LoadGameSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}