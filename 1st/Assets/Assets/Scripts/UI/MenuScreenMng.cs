using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public GameObject statsPanel;
    public GameObject settingsPanel;
    public TMP_Text gamesPlayedText;
    public TMP_Text gamesWonText;

    public Button startButton;
    public Button settingsButton;
    public Button statsButton;
    public Button exitButton;
    public Button closeStatsButton;
    public Button closeSettingsButton;

    public TMP_Dropdown qualityDropdown;
    public Slider soundSlider;
    public TMP_Text soundValueText;

    private int gamesPlayed = 0;
    private int gamesWon = 0;

    void Start()
    {
        statsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        statsButton.onClick.AddListener(OpenStats);
        exitButton.onClick.AddListener(ExitGame);
        closeStatsButton.onClick.AddListener(CloseStats);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        qualityDropdown.onValueChanged.AddListener(SetQuality);
        PopulateQualityDropdown();

        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.6f);
        UpdateSoundValueText();

        LoadStats();
    }

    public void StartGame()
    {
        gamesPlayed++; 
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed); 
        PlayerPrefs.Save(); 
        SceneManager.LoadScene("GameScene");
    }

    public void OpenStats()
    {
        statsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        UpdateStats();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        statsPanel.SetActive(false);
    }

    public void CloseStats()
    {
        statsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed);
        PlayerPrefs.SetInt("GamesWon", gamesWon);
        PlayerPrefs.Save();
        Application.Quit();
    }

    void UpdateStats()
    {
        gamesPlayedText.text = "Games Played: " + gamesPlayed.ToString();
        gamesWonText.text = "Games Won: " + gamesWon.ToString();
    }

    void LoadStats()
    {
        gamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);
        gamesWon = PlayerPrefs.GetInt("GamesWon", 0);
        UpdateStats();
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
        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
    }
}
