using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Net;
using Newtonsoft.Json.Linq;

public class GoogleAuth : MonoBehaviour
{
    private string clientId = "Enter your client ID"; 
    private string clientSecret = "Enter your client secret"; 
    private string redirectUri = "http://localhost:5000"; 

    public void OpenGoogleAuthUrl()
    {
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=email%20profile";
        Application.OpenURL(authUrl);

        StartCoroutine(StartLocalServer());
    }

    private IEnumerator StartLocalServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"{redirectUri}/");
        listener.Start();

        Debug.Log("Listening for Google Auth Response...");
        var context = listener.GetContext(); 

        string authCode = context.Request.QueryString["code"];
        listener.Stop();

        if (!string.IsNullOrEmpty(authCode))
        {
            Debug.Log("Authorization Code Received: " + authCode);
            StartCoroutine(GetAccessToken(authCode)); 
        }
        else
        {
            Debug.LogError("Authorization Code not received!");
        }

        yield break;
    }

    private IEnumerator GetAccessToken(string authCode)
    {
        string tokenUrl = "https://oauth2.googleapis.com/token";

        WWWForm form = new WWWForm();
        form.AddField("code", authCode);
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("grant_type", "authorization_code");

        UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            JObject json = JObject.Parse(www.downloadHandler.text);
            string accessToken = json["access_token"].ToString();
            Debug.Log("Access Token: " + accessToken);

            StartCoroutine(GetUserInfo(accessToken));
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    private IEnumerator GetUserInfo(string accessToken)
    {
        string userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken;
        UnityWebRequest www = UnityWebRequest.Get(userInfoUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            JObject json = JObject.Parse(www.downloadHandler.text);
            string userName = json["name"].ToString();  

            Debug.Log("User Info: " + json.ToString());
            Debug.Log("User Name: " + userName);

            OnGoogleAuthSuccess(userName); 
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    private void OnGoogleAuthSuccess(string userName)
    {
        PlayerPrefs.SetString("Username", userName);
        Debug.Log("Google Sign In successful, UserName: " + userName);
        AuthMng.LoadGameSceneByIndex(1, userName);
    }
}
