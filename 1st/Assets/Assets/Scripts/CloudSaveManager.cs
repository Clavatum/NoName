using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;
using System;
using System.Linq;

public class CloudSaveManager : MonoBehaviour
{
    public static async Task SaveToCloud(Dictionary<string, object> data)
    {
        try
        {
            Debug.Log("Saving data to cloud: " + string.Join(", ", data.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Data successfully saved to the cloud.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public static async Task<Dictionary<string, object>> LoadFromCloud()
    {
        try
        {
            var cloudData = await CloudSaveService.Instance.Data.LoadAllAsync();
            Debug.Log("Loaded Cloud Data: " + string.Join(", ", cloudData.Select(kvp => $"{kvp.Key}: {kvp.Value}")));

            var convertedData = new Dictionary<string, object>();
            foreach (var kvp in cloudData)
            {
                Debug.Log($"Converting: {kvp.Key} -> {kvp.Value}");
                if (float.TryParse(kvp.Value, out float floatValue))
                {
                    convertedData[kvp.Key] = floatValue;
                }
                else if (int.TryParse(kvp.Value, out int intValue))
                {
                    convertedData[kvp.Key] = intValue;
                }
                else
                {
                    convertedData[kvp.Key] = kvp.Value;
                }
            }
            return convertedData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return null;
        }
    }

    public static async Task InitializeNewPlayerData()
    {
        var defaultData = new Dictionary<string, object>
        {
            { "TotalGold", 500f },
            { "TotalKills", 0 },
            { "TotalPlayTime", 0f },
            { "BestCompletionTime", Mathf.Infinity },
            { "SoundVolume", 1f },
            { "QualityLevel", QualitySettings.GetQualityLevel() },
            { "GamesPlayed", 0 },
            { "GamesWon", 0 }
        };

        await SaveToCloud(defaultData);
        Debug.Log("New player data initialized.");
    }

    public static async Task ApplyCloudDataToGame()
    {
        var cloudData = await LoadFromCloud();
        if (cloudData == null) return;

        if (cloudData.TryGetValue("TotalGold", out var totalGold))
            GameStatsManager.Instance.totalGold = Convert.ToSingle(totalGold);

        if (cloudData.TryGetValue("TotalKills", out var totalKills))
            GameStatsManager.Instance.totalKills = Convert.ToInt32(totalKills);

        if (cloudData.TryGetValue("TotalPlayTime", out var totalPlayTime))
            GameStatsManager.Instance.totalPlayTime = Convert.ToSingle(totalPlayTime);

        if (cloudData.TryGetValue("BestCompletionTime", out var bestCompletionTime))
            GameStatsManager.Instance.bestCompletionTime = Convert.ToSingle(bestCompletionTime);

        if (cloudData.TryGetValue("SoundVolume", out var soundVolume))
            AudioListener.volume = Convert.ToSingle(soundVolume);

        if (cloudData.TryGetValue("QualityLevel", out var qualityLevel))
            QualitySettings.SetQualityLevel(Convert.ToInt32(qualityLevel));

        if (cloudData.TryGetValue("GamesPlayed", out var gamesPlayed))
            GameStatsManager.Instance.gamesPlayed = Convert.ToInt32(gamesPlayed); 
    }

    public static Dictionary<string, object> CollectDataForSave()
    {
        return new Dictionary<string, object>
        {
            { "TotalGold", GameStatsManager.Instance.totalGold },
            { "TotalKills", GameStatsManager.Instance.totalKills },
            { "TotalPlayTime", GameStatsManager.Instance.totalPlayTime },
            { "BestCompletionTime", GameStatsManager.Instance.bestCompletionTime },
            { "SoundVolume", PlayerPrefs.GetFloat("SoundVolume", 0.6f) },
            { "QualityLevel", PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel()) },
            { "GamesPlayed", GameStatsManager.Instance.gamesPlayed } 
        };
    }
}
