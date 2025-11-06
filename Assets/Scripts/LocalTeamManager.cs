using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class LocalPlayerData
{
    public string playerName;
    public int teamId; // 0 = no team, 1 = team 1, 2 = team 2
    
    public LocalPlayerData(string name)
    {
        playerName = name;
        teamId = 0;
    }
}

public class LocalTeamManager : MonoBehaviour
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
    
    private List<LocalPlayerData> allPlayers = new List<LocalPlayerData>();
    private List<LocalPlayerData> team1Players = new List<LocalPlayerData>();
    private List<LocalPlayerData> team2Players = new List<LocalPlayerData>();
    
    private const int MAX_TEAM_SIZE = 2;
    private string currentPlayerName = "Player1"; // For testing
    
    void Start()
    {
        SetupUI();
        
        // Add some test players
        AddPlayer("Player1");
        AddPlayer("Player2");
        AddPlayer("Player3");
        AddPlayer("Player4");
    }
    
    void SetupUI()
    {
        joinTeam1Button.onClick.AddListener(() => JoinTeam(1));
        joinTeam2Button.onClick.AddListener(() => JoinTeam(2));
        playButton.onClick.AddListener(StartGame);
        
        UpdateUI();
    }
    
    public void AddPlayer(string playerName)
    {
        LocalPlayerData newPlayer = new LocalPlayerData(playerName);
        allPlayers.Add(newPlayer);
        
        UpdateUI();
    }
    
    public void RemovePlayer(string playerName)
    {
        // Remove from all players list
        allPlayers.RemoveAll(p => p.playerName == playerName);
        
        // Remove from team lists
        team1Players.RemoveAll(p => p.playerName == playerName);
        team2Players.RemoveAll(p => p.playerName == playerName);
        
        UpdateUI();
    }
    
    void JoinTeam(int teamId)
    {
        LocalPlayerData player = allPlayers.Find(p => p.playerName == currentPlayerName);
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
    }
    
    void UpdateUI()
    {
        // Update team displays
        string team1Text = "Equipo 1:\n";
        foreach (var player in team1Players)
        {
            team1Text += $"• {player.playerName}\n";
        }
        if (team1Players.Count == 0)
        {
            team1Text += "(Vacío)";
        }
        team1PlayersText.text = team1Text;
        
        string team2Text = "Equipo 2:\n";
        foreach (var player in team2Players)
        {
            team2Text += $"• {player.playerName}\n";
        }
        if (team2Players.Count == 0)
        {
            team2Text += "(Vacío)";
        }
        team2PlayersText.text = team2Text;
        
        // Update button states
        joinTeam1Button.interactable = team1Players.Count < MAX_TEAM_SIZE;
        joinTeam2Button.interactable = team2Players.Count < MAX_TEAM_SIZE;
        
        // Update play button
        bool canPlay = team1Players.Count == MAX_TEAM_SIZE && team2Players.Count == MAX_TEAM_SIZE;
        playButton.interactable = canPlay;
        
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
        if (team1Players.Count == MAX_TEAM_SIZE && team2Players.Count == MAX_TEAM_SIZE)
        {
            // Load game scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }
    
    // Method to simulate different players joining teams (for testing)
    public void SimulatePlayerJoinTeam(string playerName, int teamId)
    {
        string previousPlayer = currentPlayerName;
        currentPlayerName = playerName;
        JoinTeam(teamId);
        currentPlayerName = previousPlayer;
    }
}