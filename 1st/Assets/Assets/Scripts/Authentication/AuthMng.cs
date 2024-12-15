using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication.PlayerAccounts;
using System;
using System.Runtime.CompilerServices;

public class AuthMng : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text logTxt;

    public Button loginButton;

    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private LoginController loginController;

    private PlayerProfile playerProfile;
    public event Action<PlayerProfile> OnSignedIn;
    public PlayerProfile PlayerProfile => playerProfile;

    private GameStatsManager gameStatsManager;
    private PlayerInfo playerInfo;

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        gameStatsManager = GameStatsManager.Instance;

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
            LoadGameSceneByIndex(1, username);

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
            LoadGameSceneByIndex(1, username);

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

    public static void LoadGameSceneByIndex(int sceneIndex, string username)
    {
        PlayerPrefs.SetString("Username", username);
        SceneManager.LoadScene(sceneIndex);
    }

    #region - UnityAccount -

    private void OnEnable()
    {
        loginController.OnSignedIn += LoginController_OnSignedIn;
    }

    private void OnDisable()
    {
        loginController.OnSignedIn -= LoginController_OnSignedIn;
    }

    public async void LoginButtonPressed()
    {
        await loginController.InitSignIn();
        LoadGameSceneByIndex(1, playerProfile.Name);
    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        if(userNameText == null) { return; }
        userNameText.text = profile.Name;
    }
    #endregion
}