using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI and General Sounds")]
    public AudioClip gameOverSound;
    public AudioClip gameWinSound;

    [Header("Player Sounds")]
    public AudioClip walkingFootstepSound;
    public AudioClip runningFootstepSound;
    public AudioClip swordSwingSound;
    public AudioClip playerAttackSound;
    public AudioClip femaleImpactSound;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    public AudioClip monsterSound;

    public AudioSource musicSource;
    public AudioSource effectsSource;
    private Scene currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene;

        if (currentScene.name == "GameScene")
        {
            StopBackgroundMusic();
        }
        else
        {
            PlayBackgroundMusic();
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
        effectsSource.PlayOneShot(clip);

        /*if (clip != null && !effectsSource.isPlaying)
        {
            effectsSource.PlayOneShot(clip);
            return;
        }
        else
        {
            Debug.LogWarning("Sound effect clip is not set or already playing!");
        }*/
        return;
    }

    public void PlayGameOverSound() => PlaySoundEffect(gameOverSound);
    public void PlayFemaleImpactSound() => PlaySoundEffect(femaleImpactSound);
    public void PlayMonsterSound() => PlaySoundEffect(monsterSound);
    public void PlayGameWinSound() => PlaySoundEffect(gameWinSound);
    public void PlayWalkingFootStepSound() => PlaySoundEffect(walkingFootstepSound);
    public void PlayRunningFootstepSound() => PlaySoundEffect(runningFootstepSound);
    public void PlaySwordSwingSound() => PlaySoundEffect(swordSwingSound);
    public void PlayPlayerAttackSound() => PlaySoundEffect(playerAttackSound);
}
