using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PausePanelMng : MonoBehaviour
{
    UIInputActions UIInputs;

    [Header("UI Elements")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public TextMeshProUGUI countdownText;
    public TMP_Dropdown qualityDropdown;
    public Slider soundSlider;
    public TextMeshProUGUI soundValueText;

    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button exitButton;
    public Button closeSettingsButton;

    public bool isPaused = false;
    private float countdownDuration = 3f;
    private float countdownTimer;

    private void Awake()
    {
        UIInputs = new UIInputActions();

        // Keep your existing input system setup for pausing
        UIInputs.Actions.Pause.performed += e => SetPauseFlag();
        UIInputs.Enable();
    }

    void Start()
    {
        // Initialize buttons with their respective event listeners
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitToMenu);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        // Initialize settings panel elements
        settingsPanel.SetActive(false);
        PopulateQualityDropdown();

        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.6f);
        UpdateSoundValueText();

        // Hide countdown text at the start
        countdownText.gameObject.SetActive(false);

        // Ensure the game starts unpaused
        Time.timeScale = 1;
    }

    void Update()
    {
        // When paused, activate the pause panel
        if (isPaused)
        {
            PauseGame();
        }
    }

    void SetPauseFlag()
    {
        isPaused = true; // Pause triggered by the input action
    }

    void PauseGame()
    {
        // Ensure the game is paused properly
        Time.timeScale = 0;
        if (!settingsPanel.activeSelf) // Pause panel should only be visible when settings are not active
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        /*// Only resume if the game is actually paused
        if (isPaused)
        {
            pausePanel.SetActive(false); // Deactivate pause panel immediately
            countdownText.gameObject.SetActive(true); // Activate countdown text
            StartCoroutine(ResumeAfterCountdown());
        }*/
        isPaused = false;
        pausePanel.SetActive(false); // Deactivate pause panel immediately
        countdownText.gameObject.SetActive(true); // Activate countdown text
        StartCoroutine(ResumeAfterCountdown());
    }

    private IEnumerator ResumeAfterCountdown()
    {
        // Start countdown
        countdownTimer = countdownDuration;

        while (countdownTimer > 0)
        {
            countdownText.text = "Resuming in " + countdownTimer.ToString("F0") + "...";
            yield return new WaitForSecondsRealtime(1f); // Use unscaled time to avoid dependency on timeScale
            countdownTimer--;
        }

        // Clear countdown text, hide it, and resume the game
        countdownText.text = "";
        countdownText.gameObject.SetActive(false); // Deactivate countdown text
        Time.timeScale = 1; // Resume the game
        isPaused = false; // Reset the pause flag to indicate the game is no longer paused
    }

    public void RestartGame()
    {
        // Ensure timeScale is reset before restarting
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void OpenSettings()
    {
        // Show settings and hide the pause panel
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false); // Deactivate pause panel when opening settings
    }

    public void CloseSettings()
    {
        // Close settings and show the pause panel again if the game is still paused
        settingsPanel.SetActive(false);
        if (isPaused)
        {
            pausePanel.SetActive(true); // Show pause panel again when settings are closed
        }
    }

    public void ExitToMenu()
    {
        // Ensure timeScale is reset before exiting to the menu
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu"); // Load Menu scene
    }

    // Settings functions

    void PopulateQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<string> qualityNames = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityNames);

        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetSoundVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
        UpdateSoundValueText();
    }

    void UpdateSoundValueText()
    {
        soundValueText.text = (soundSlider.value * 100).ToString("0") + "%";
    }

    private void OnDestroy()
    {
        // Unbind event listeners to avoid memory leaks
        resumeButton.onClick.RemoveListener(ResumeGame);
        restartButton.onClick.RemoveListener(RestartGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        exitButton.onClick.RemoveListener(ExitToMenu);
        closeSettingsButton.onClick.RemoveListener(CloseSettings);

        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        UIInputs.Enable();
    }

    private void OnDisable()
    {
        UIInputs.Disable();
    }

    #endregion
}
