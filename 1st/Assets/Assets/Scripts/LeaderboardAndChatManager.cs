using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardAndChatManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    [SerializeField] private GameObject leaderboardContent;
    [SerializeField] private GameObject leaderboardRowPrefab;
    [SerializeField] private List<PlayerData> players;

    [Header("Chat UI")]
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private TMP_Text chatDisplay;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button closeChatPanelButton;

    private string currentChatKey = "";

    private void Start()
    {
        InitializeLeaderboard();
        chatPanel.SetActive(false);

        sendButton.onClick.AddListener(SendMessage);
        closeChatPanelButton.onClick.AddListener(CloseChat);
    }

    private void InitializeLeaderboard()
    {
        players.Sort((a, b) => b.killScore.CompareTo(a.killScore));

        foreach (var player in players)
        {
            var row = Instantiate(leaderboardRowPrefab, leaderboardContent.transform);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = player.playerName;
            texts[1].text = "Kills: " + player.killScore.ToString();

            Button playerButton = row.GetComponent<Button>();
            playerButton.onClick.AddListener(() => OpenChat(player));
        }
    }

    private void OpenChat(PlayerData player)
    {
        currentChatKey = $"Chat_{player.playerName}";
        chatPanel.SetActive(true);
        chatDisplay.text = LoadMessages(currentChatKey);
        Debug.Log($"Opened chat with {player.playerName}");
    }

    private void CloseChat()
    {
        chatPanel.SetActive(false);
    }

    private void SendMessage()
    {
        string message = chatInput.text;
        if (string.IsNullOrWhiteSpace(message))
        {
            Debug.LogWarning("Message is empty, nothing to send.");
            return;
        }

        chatDisplay.text += $"\nYou: {message}";
        chatInput.text = "";

        SaveMessage(currentChatKey, $"You: {message}");
        Debug.Log($"Message saved for {currentChatKey}");
    }

    private void SaveMessage(string key, string message)
    {
        string existingMessages = PlayerPrefs.GetString(key, "");
        existingMessages += $"\n{message}";
        PlayerPrefs.SetString(key, existingMessages);
        PlayerPrefs.Save();
    }

    private string LoadMessages(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int killScore;
}
