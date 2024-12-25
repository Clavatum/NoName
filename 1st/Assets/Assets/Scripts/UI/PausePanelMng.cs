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

    private bool settingsChanged = false;

    private void Awake()
    {
        UIInputs = new UIInputActions();

        UIInputs.Actions.Pause.performed += e => SetPauseFlag();
        UIInputs.Enable();
    }

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitToMenu);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        settingsPanel.SetActive(false);
        PopulateQualityDropdown();

        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.6f);
        UpdateSoundValueText();

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1;
    }

    void Update()
    {
        if (isPaused)
        {
            PauseGame();
        }
    }

    void SetPauseFlag()
    {
        isPaused = true;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        if (!settingsPanel.activeSelf)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayButtonClick();
        isPaused = false;
        pausePanel.SetActive(false);
        countdownText.gameObject.SetActive(true);
        StartCoroutine(ResumeAfterCountdown());
    }

    private IEnumerator ResumeAfterCountdown()
    {
        countdownTimer = countdownDuration;

        while (countdownTimer > 0)
        {
            countdownText.text = "Resuming in " + countdownTimer.ToString("F0") + "...";
            yield return new WaitForSecondsRealtime(1f);
            countdownTimer--;
        }

        countdownText.text = "";
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlayButtonClick();
        GameStatsManager.Instance.StopGamePlayTime();
        GameStatsManager.Instance.StartGamePlayTime();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenSettings()
    {
        AudioManager.Instance.PlayButtonClick();
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void CloseSettings()
    {
        AudioManager.Instance.PlayButtonClick();
        settingsPanel.SetActive(false);
        if (isPaused)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ExitToMenu()
    {
        MenuScreenManager.hasComebackToMenu = true;
        AudioManager.Instance.PlayButtonClick();
        GameStatsManager.Instance.StopGamePlayTime();
        if (settingsChanged)
        {
            SaveSettings();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

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
        AudioManager.Instance.PlayButtonClick();
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        settingsChanged = true;
    }

    public void SetSoundVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
        UpdateSoundValueText();
        settingsChanged = true;
    }

    void UpdateSoundValueText()
    {
        soundValueText.text = (soundSlider.value * 100).ToString("0") + "%";
    }

    private void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved.");
    }

    private void OnDestroy()
    {
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
