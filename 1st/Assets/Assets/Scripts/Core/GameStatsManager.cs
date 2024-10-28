using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    public int totalKills = 0;         // Öldürülen düþman sayýsý
    public float goldSpent = 0f;       // Harcanan altýn
    public float goldEarned = 0f;      // Kazanýlan altýn
    public float totalGold = 0f;       // Toplam altýn (kalýcý)

    public float completionTime = 0f;  // Bu oyun için tamamlanma süresi
    public float bestCompletionTime = Mathf.Infinity; // En kýsa tamamlanma süresi
    public float totalPlayTime = 0f;   // Toplam oynama süresi (kalýcý)

    private float startTime;           // Oyun baþlangýç zamaný

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameStats();  // Oyun istatistiklerini yükle
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startTime = Time.time;  // Oyun baþlangýç zamaný
    }

    public void AddKill()
    {
        totalKills++;
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
        completionTime = Time.time - startTime;  // Oyun süresi
        totalPlayTime += completionTime;         // Toplam oynama süresi artar

        // En kýsa tamamlanma süresi güncellenir
        if (completionTime < bestCompletionTime)
        {
            bestCompletionTime = completionTime;
        }

        // Altýn ve diðer deðerler kaydedilir
        SaveGameStats();
    }

    // Ýstatistikleri PlayerPrefs ile kaydetme
    public void SaveGameStats()
    {
        PlayerPrefs.SetFloat("TotalGold", totalGold);
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.SetFloat("BestCompletionTime", bestCompletionTime);
        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.Save();
    }

    // Ýstatistikleri yükleme
    public void LoadGameStats()
    {
        totalGold = PlayerPrefs.GetFloat("TotalGold", 0f);
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        bestCompletionTime = PlayerPrefs.GetFloat("BestCompletionTime", Mathf.Infinity);
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);
    }
}
