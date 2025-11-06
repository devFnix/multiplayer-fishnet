using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;

[System.Serializable]
public class RoomData
{
    public string roomName;
    public int currentPlayers;
    public int maxPlayers;
    public string roomId;
    
    public RoomData(string name, int current, int max, string id)
    {
        roomName = name;
        currentPlayers = current;
        maxPlayers = max;
        roomId = id;
    }
}

public class RoomItem : MonoBehaviour
{
    [Header("UI References")]
    public Text roomNameText;
    public Text playerCountText;
    public Button joinButton;
    
    private RoomData roomData;
    private SimpleRoomManager roomManager;
    
    public void Initialize(RoomData data, SimpleRoomManager manager)
    {
        roomData = data;
        roomManager = manager;
        
        UpdateUI();
        
        // Setup button listener
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }
    
    void UpdateUI()
    {
        if (roomData == null) return;
        
        roomNameText.text = roomData.roomName;
        playerCountText.text = $"👥 {roomData.currentPlayers}/{roomData.maxPlayers}";
        
        // Enable/disable join button based on room capacity
        bool canJoin = roomData.currentPlayers < roomData.maxPlayers;
        joinButton.interactable = canJoin;
        
        // Change button color based on availability
        var colors = joinButton.colors;
        if (canJoin)
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        }
        else
        {
            colors.normalColor = new Color(0.7f, 0.7f, 0.7f);
            colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f);
        }
        joinButton.colors = colors;
    }
    
    void OnJoinButtonClicked()
    {
        if (roomManager != null && roomData != null)
        {
            roomManager.JoinRoom(roomData.roomId);
            roomManager.JoinRoomAndShowTeams(roomData);
        }
    }
    
    public void UpdateRoomData(RoomData newData)
    {
        roomData = newData;
        UpdateUI();
    }
}