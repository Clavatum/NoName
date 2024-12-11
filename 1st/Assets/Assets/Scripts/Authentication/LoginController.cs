using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;

    private PlayerInfo playerInfo;
    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignedIn;
    }

    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;
            PlayerPrefs.SetString("Username", name);

            OnSignedIn?.Invoke(playerProfile);

            var cloudData = await CloudSaveManager.LoadFromCloud();
            if (cloudData == null || cloudData.Count == 0)
            {
                Debug.Log("New player detected. Initializing stats.");
                await CloudSaveManager.InitializeNewPlayerData();
                await CloudSaveManager.ApplyCloudDataToGame();
            }
            else
            {
                Debug.Log("Returning player detected. Loading stats.");
                await CloudSaveManager.ApplyCloudDataToGame();
            }
            Debug.Log("g");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}

[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}
