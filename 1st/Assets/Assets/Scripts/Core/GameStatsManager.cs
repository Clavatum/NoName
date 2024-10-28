using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    public int totalKills = 0;         
    public int totalKillsInGame = 0;         
    public float goldSpent = 0f;       
    public float goldEarned = 0f;      
    public float totalGold = 0f;       

    public float completionTime = 0f;  
    public float bestCompletionTime = Mathf.Infinity;
    public float totalPlayTime = 0f;   

    private float startTime;           

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameStats();  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startTime = Time.time;  
    }

    public void AddKill()
    {
        totalKills++;
        totalKillsInGame++;
    }

    public void SpendGold(float amount)
    {
        goldSpent += amount;
        totalGold -= amount;
    }

    public void EarnGold(float amount)
    {
        goldEarned += amount;
        totalGold += amount;
    }

    public void CompleteGame()
    {
        completionTime = Time.time - startTime; 
        totalPlayTime += completionTime;         

        if (completionTime < bestCompletionTime)
        {
            bestCompletionTime = completionTime;
        }

        SaveGameStats();
    }

    public void SaveGameStats()
    {
        PlayerPrefs.SetFloat("TotalGold", totalGold);
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.SetFloat("BestCompletionTime", bestCompletionTime);
        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.Save();
    }

    public void LoadGameStats()
    {
        totalGold = PlayerPrefs.GetFloat("TotalGold", 0f);
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        bestCompletionTime = PlayerPrefs.GetFloat("BestCompletionTime", Mathf.Infinity);
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);
    }
}
