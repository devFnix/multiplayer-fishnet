using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleRoomManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject roomPanel;
    public GameObject mainMenuPanel;
    public GameObject createRoomPopup;
    public GameObject teamSelectionPanel;
    public Button createRoomButton;
    public Button backToMenuButton;
    public InputField roomNameInput;
    public Button confirmButton;
    public Button cancelButton;
    public Transform roomListContent;
    public GameObject roomItemPrefab;
    
    [Header("Team Selection UI")]
    public Button joinTeam1Button;
    public Button joinTeam2Button;
    public Button playButton;
    public Button backToRoomListButton;
    public Text team1PlayersText;
    public Text team2PlayersText;
    public Text gameStatusText;
    public Text roomTitleInTeamSelection;
    
    [Header("Network References")]
    public MultiplayerMenuManager menuManager;
    public TeamManager teamManager;
    
    private List<RoomData> rooms = new List<RoomData>();
    private List<GameObject> roomItems = new List<GameObject>();
    private string currentPlayerName = "";
    
    void Start()
    {
        SetupUI();
    }
    
    void SetupUI()
    {
        // Setup button listeners
        createRoomButton.onClick.AddListener(ShowCreateRoomPopup);
        backToMenuButton.onClick.AddListener(BackToMainMenu);
        confirmButton.onClick.AddListener(CreateRoom);
        cancelButton.onClick.AddListener(HideCreateRoomPopup);
        backToRoomListButton.onClick.AddListener(BackToRoomList);
        
        // Initially hide panels and popup
        roomPanel.SetActive(false);
        createRoomPopup.SetActive(false);
        teamSelectionPanel.SetActive(false);
    }
    
    public void ShowRoomPanel(string playerName)
    {
        currentPlayerName = playerName;
        mainMenuPanel.SetActive(false);
        roomPanel.SetActive(true);
        
        // Create some example rooms for demonstration
        CreateExampleRooms();
    }
    
    void CreateExampleRooms()
    {
        // Clear existing rooms
        ClearRoomList();
        
        // Create example rooms
        rooms.Add(new RoomData("Sala Principal", 2, 4, "room1"));
        rooms.Add(new RoomData("Partida Rápida", 1, 4, "room2"));
        rooms.Add(new RoomData("Sala VIP", 4, 4, "room3"));
        
        RefreshRoomList();
    }
    
    void ShowCreateRoomPopup()
    {
        createRoomPopup.SetActive(true);
        roomNameInput.text = "";
        roomNameInput.Select();
    }
    
    void HideCreateRoomPopup()
    {
        createRoomPopup.SetActive(false);
    }
    
    void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("Room name cannot be empty");
            return;
        }
        
        HideCreateRoomPopup();
        
        // Create new room
        string roomId = System.Guid.NewGuid().ToString();
        RoomData newRoom = new RoomData(roomName, 1, 4, roomId);
        rooms.Add(newRoom);
        
        RefreshRoomList();
        
        Debug.Log($"Room created: {roomName}");
    }
    
    void RefreshRoomList()
    {
        // Clear existing room items
        ClearRoomList();
        
        // Create room items
        foreach (RoomData room in rooms)
        {
            GameObject newRoomItem = Instantiate(roomItemPrefab, roomListContent);
            RoomItem roomItem = newRoomItem.GetComponent<RoomItem>();
            roomItem.Initialize(room, this);
            
            roomItems.Add(newRoomItem);
        }
    }
    
    void ClearRoomList()
    {
        foreach (GameObject roomItem in roomItems)
        {
            if (roomItem != null)
                Destroy(roomItem);
        }
        roomItems.Clear();
    }
    
    public void JoinRoom(string roomId)
    {
        RoomData room = rooms.Find(r => r.roomId == roomId);
        if (room == null)
        {
            Debug.LogWarning($"Room {roomId} not found");
            return;
        }
        
        if (room.currentPlayers >= room.maxPlayers)
        {
            Debug.LogWarning($"Room {roomId} is full");
            return;
        }
        
        // Update room player count
        room.currentPlayers++;
        
        // Refresh the room list to update UI
        RefreshRoomList();
        
        Debug.Log($"Joined room: {room.roomName}. Players: {room.currentPlayers}/{room.maxPlayers}");
    }
    
    void BackToMainMenu()
    {
        roomPanel.SetActive(false);
        teamSelectionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        // Clear room list
        ClearRoomList();
        rooms.Clear();
        
        // Disconnect from network
        if (menuManager != null)
        {
            menuManager.Disconnect();
        }
    }
    
    void BackToRoomList()
    {
        teamSelectionPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
    
    public void JoinRoomAndShowTeams(RoomData roomData)
    {
        // Show team selection panel
        roomPanel.SetActive(false);
        teamSelectionPanel.SetActive(true);
        
        // Update room title in team selection
        if (roomTitleInTeamSelection != null)
        {
            roomTitleInTeamSelection.text = roomData.roomName;
        }
        
        // Initialize team manager if available
        if (teamManager != null)
        {
            teamManager.AddPlayer(currentPlayerName, null); // In real implementation, pass actual connection
        }
        
        Debug.Log($"Joined room: {roomData.roomName}");
    }
}