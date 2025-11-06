using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public NetworkConnection connection;
    public int teamId; // 0 = no team, 1 = team 1, 2 = team 2
    
    public PlayerData(string name, NetworkConnection conn)
    {
        playerName = name;
        connection = conn;
        teamId = 0;
    }
}

public class TeamManager : NetworkBehaviour
{
    [Header("UI References")]
    public GameObject teamSelectionPanel;
    public Button joinTeam1Button;
    public Button joinTeam2Button;
    public Button playButton;
    public Text team1PlayersText;
    public Text team2PlayersText;
    public Text gameStatusText;
    
    [Header("Game Settings")]
    public string gameSceneName = "GameScene";
    
    private List<PlayerData> allPlayers = new List<PlayerData>();
    private List<PlayerData> team1Players = new List<PlayerData>();
    private List<PlayerData> team2Players = new List<PlayerData>();
    
    private const int MAX_TEAM_SIZE = 2;
    
    void Start()
    {
        if (IsServer)
        {
            SetupServerUI();
        }
        else
        {
            SetupClientUI();
        }
    }
    
    void SetupServerUI()
    {
        joinTeam1Button.onClick.AddListener(() => RequestJoinTeam(1));
        joinTeam2Button.onClick.AddListener(() => RequestJoinTeam(2));
        playButton.onClick.AddListener(StartGame);
        
        UpdateUI();
    }
    
    void SetupClientUI()
    {
        joinTeam1Button.onClick.AddListener(() => RequestJoinTeam(1));
        joinTeam2Button.onClick.AddListener(() => RequestJoinTeam(2));
        
        // Clients can't start the game directly
        playButton.interactable = false;
        
        UpdateUI();
    }
    
    public void AddPlayer(string playerName, NetworkConnection connection)
    {
        if (!IsServer) return;
        
        PlayerData newPlayer = new PlayerData(playerName, connection);
        allPlayers.Add(newPlayer);
        
        UpdateUI();
    }
    
    public void RemovePlayer(NetworkConnection connection)
    {
        if (!IsServer) return;
        
        // Remove from all players list
        allPlayers.RemoveAll(p => p.connection == connection);
        
        // Remove from team lists
        team1Players.RemoveAll(p => p.connection == connection);
        team2Players.RemoveAll(p => p.connection == connection);
        
        UpdateUI();
    }
    
    void RequestJoinTeam(int teamId)
    {
        if (IsServer)
        {
            JoinTeam(base.Owner, teamId);
        }
        else
        {
            JoinTeamServerRpc(teamId);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void JoinTeamServerRpc(int teamId, NetworkConnection sender = null)
    {
        JoinTeam(sender, teamId);
    }
    
    void JoinTeam(NetworkConnection connection, int teamId)
    {
        if (!IsServer) return;
        
        PlayerData player = allPlayers.Find(p => p.connection == connection);
        if (player == null) return;
        
        // Remove player from current team
        if (player.teamId == 1)
        {
            team1Players.Remove(player);
        }
        else if (player.teamId == 2)
        {
            team2Players.Remove(player);
        }
        
        // Check if target team has space
        if (teamId == 1 && team1Players.Count < MAX_TEAM_SIZE)
        {
            player.teamId = 1;
            team1Players.Add(player);
        }
        else if (teamId == 2 && team2Players.Count < MAX_TEAM_SIZE)
        {
            player.teamId = 2;
            team2Players.Add(player);
        }
        else
        {
            // Team is full, keep player in no team
            player.teamId = 0;
        }
        
        UpdateUI();
        UpdateUIClientRpc();
    }
    
    [ObserversRpc]
    void UpdateUIClientRpc()
    {
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // Update team displays
        string team1Text = "Equipo 1:\n";
        foreach (var player in team1Players)
        {
            team1Text += $"• {player.playerName}\n";
        }
        team1PlayersText.text = team1Text;
        
        string team2Text = "Equipo 2:\n";
        foreach (var player in team2Players)
        {
            team2Text += $"• {player.playerName}\n";
        }
        team2PlayersText.text = team2Text;
        
        // Update button states
        joinTeam1Button.interactable = team1Players.Count < MAX_TEAM_SIZE;
        joinTeam2Button.interactable = team2Players.Count < MAX_TEAM_SIZE;
        
        // Update play button
        bool canPlay = team1Players.Count == MAX_TEAM_SIZE && team2Players.Count == MAX_TEAM_SIZE;
        playButton.interactable = canPlay && IsServer;
        
        if (canPlay)
        {
            gameStatusText.text = "¡Listo para jugar! 2 vs 2";
        }
        else
        {
            gameStatusText.text = $"Esperando jugadores... ({team1Players.Count + team2Players.Count}/4)";
        }
    }
    
    void StartGame()
    {
        if (!IsServer) return;
        
        if (team1Players.Count == MAX_TEAM_SIZE && team2Players.Count == MAX_TEAM_SIZE)
        {
            // Load game scene for all players
            LoadGameSceneClientRpc();
        }
    }
    
    [ObserversRpc]
    void LoadGameSceneClientRpc()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
}