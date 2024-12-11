using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardAndChatManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    [SerializeField] private GameObject leaderboardContent; // Content holder for leaderboard rows
    [SerializeField] private GameObject leaderboardRowPrefab; // Prefab for player rows
    [SerializeField] private List<PlayerData> players; // Player data list

    [Header("Chat UI")]
    [SerializeField] private GameObject chatPanel; // Chat panel object
    [SerializeField] private TMP_Text chatDisplay; // Text area to display messages
    [SerializeField] private TMP_InputField chatInput; // Input field for messages
    [SerializeField] private Button sendButton; // Button to send messages
    [SerializeField] private Button closeChatPanelButton;

    private string currentChatKey = ""; // Key to track the selected player's chat

    private void Start()
    {
        InitializeLeaderboard();
        chatPanel.SetActive(false); // Hide chat panel initially

        sendButton.onClick.AddListener(SendMessage);
        closeChatPanelButton.onClick.AddListener(CloseChat);
    }

    private void InitializeLeaderboard()
    {
        // Sort players by kill score in descending order
        players.Sort((a, b) => b.killScore.CompareTo(a.killScore));

        foreach (var player in players)
        {
            var row = Instantiate(leaderboardRowPrefab, leaderboardContent.transform);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = player.playerName; // Player name
            texts[1].text = "Kills: " + player.killScore.ToString(); // Kill score

            Button playerButton = row.GetComponent<Button>();
            playerButton.onClick.AddListener(() => OpenChat(player));
        }
    }

    private void OpenChat(PlayerData player)
    {
        currentChatKey = $"Chat_{player.playerName}";
        chatPanel.SetActive(true); // Show chat panel
        chatDisplay.text = LoadMessages(currentChatKey); // Load messages for this player
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

        // Display message
        chatDisplay.text += $"\nYou: {message}";
        chatInput.text = ""; // Clear input field

        // Save message locally
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
    public string playerName; // Player name
    public int killScore; // Kill score
}
