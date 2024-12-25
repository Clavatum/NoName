using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI and General Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip gameOverSound;
    public AudioClip gameWinSound;

    [Header("Player Sounds")]
    public AudioClip walkingFootstepSound;
    public AudioClip runningFootstepSound;
    public AudioClip swordSwingSound;
    public AudioClip playerAttackSound;

    [Header("Game Mechanic Sounds")]
    public AudioClip updateTowerSound;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    public AudioSource musicSource;
    //public AudioSource effectsSource;

    private MenuScreenManager menuScreenManager;
    private Scene currentScene;

    //public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // DontDestroyOnLoad(audioSource);
        }
        else
        {
            Destroy(gameObject);
        }

        /*musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.04f;

        effectsSource = gameObject.AddComponent<AudioSource>();
        effectsSource.playOnAwake = false;*/

        // Register for scene loading callback
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene;

        if (currentScene.name != "Menu")
        {
            StopBackgroundMusic();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music clip is not set or already playing!");
        }
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        /*if (clip != null && !effectsSource.isPlaying)
        {
            effectsSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Sound effect clip is not set or already playing!");
        }*/
        return;
    }

    public void PlayButtonClick() => PlaySoundEffect(buttonClickSound);
    public void PlayGameOverSound() => PlaySoundEffect(gameOverSound);
    public void PlayGameWinSound() => PlaySoundEffect(gameWinSound);
    public void PlayWalkingFootStepSound() => PlaySoundEffect(walkingFootstepSound);
    public void PlayRunningFootstepSound() => PlaySoundEffect(runningFootstepSound);
    public void PlaySwordSwingSound() => PlaySoundEffect(swordSwingSound);
    public void PlayPlayerAttackSound() => PlaySoundEffect(playerAttackSound);
    public void PlayUpdateTowerSound() => PlaySoundEffect(updateTowerSound);
}
