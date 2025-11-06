using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;

[System.Serializable]
public class GamePlayer
{
    public string playerName;
    public NetworkConnection connection;
    public int teamId;
    public int playerIndex; // 0 or 1 within the team
    
    public GamePlayer(string name, NetworkConnection conn, int team, int index)
    {
        playerName = name;
        connection = conn;
        teamId = team;
        playerIndex = index;
    }
}

public class TurnBasedGameManager : NetworkBehaviour
{
    [Header("UI References")]
    public Text currentTurnText;
    public Text gameInfoText;
    public Button[] gameButtons; // Interactive buttons for the game
    public GameObject gameUI;
    
    [Header("Game Settings")]
    public int totalButtons = 6;
    
    private List<GamePlayer> team1Players = new List<GamePlayer>();
    private List<GamePlayer> team2Players = new List<GamePlayer>();
    private List<GamePlayer> allPlayers = new List<GamePlayer>();
    
    private int currentPlayerIndex = 0;
    private GamePlayer currentPlayer;
    private bool gameStarted = false;
    
    // Turn order: Team1Player1 -> Team2Player1 -> Team1Player2 -> Team2Player2 -> repeat
    private List<GamePlayer> turnOrder = new List<GamePlayer>();
    
    void Start()
    {
        if (IsServer)
        {
            InitializeGame();
        }
        
        SetupGameButtons();
    }
    
    void InitializeGame()
    {
        if (!IsServer) return;
        
        // In a real implementation, you would get this data from the previous scene
        // For now, we'll simulate it
        CreateTestPlayers();
        
        SetupTurnOrder();
        StartGameRound();
    }
    
    void CreateTestPlayers()
    {
        // This is just for testing - in real implementation, get from TeamManager
        team1Players.Add(new GamePlayer("Player1", null, 1, 0));
        team1Players.Add(new GamePlayer("Player2", null, 1, 1));
        team2Players.Add(new GamePlayer("Player3", null, 2, 0));
        team2Players.Add(new GamePlayer("Player4", null, 2, 1));
        
        allPlayers.AddRange(team1Players);
        allPlayers.AddRange(team2Players);
    }
    
    void SetupTurnOrder()
    {
        turnOrder.Clear();
        
        // Turn order: Team1Player1 -> Team2Player1 -> Team1Player2 -> Team2Player2
        if (team1Players.Count >= 1) turnOrder.Add(team1Players[0]);
        if (team2Players.Count >= 1) turnOrder.Add(team2Players[0]);
        if (team1Players.Count >= 2) turnOrder.Add(team1Players[1]);
        if (team2Players.Count >= 2) turnOrder.Add(team2Players[1]);
    }
    
    void SetupGameButtons()
    {
        // Create buttons if they don't exist
        if (gameButtons == null || gameButtons.Length == 0)
        {
            CreateGameButtons();
        }
        
        // Setup button listeners
        for (int i = 0; i < gameButtons.Length; i++)
        {
            int buttonIndex = i; // Capture for closure
            gameButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));
        }
    }
    
    void CreateGameButtons()
    {
        // This would typically be done in the editor, but here's how to do it programmatically
        GameObject buttonParent = GameObject.Find("ButtonContainer");
        if (buttonParent == null)
        {
            buttonParent = new GameObject("ButtonContainer");
            buttonParent.transform.SetParent(gameUI.transform);
        }
        
        gameButtons = new Button[totalButtons];
        
        for (int i = 0; i < totalButtons; i++)
        {
            GameObject buttonObj = new GameObject($"GameButton_{i}");
            buttonObj.transform.SetParent(buttonParent.transform);
            
            Button button = buttonObj.AddComponent<Button>();
            Image image = buttonObj.AddComponent<Image>();
            
            // Position buttons in a grid
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 50);
            rect.anchoredPosition = new Vector2((i % 3) * 120 - 120, -(i / 3) * 70);
            
            // Add text to button
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            Text text = textObj.AddComponent<Text>();
            text.text = $"Button {i + 1}";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            gameButtons[i] = button;
        }
    }
    
    void StartGameRound()
    {
        if (!IsServer) return;
        
        gameStarted = true;
        currentPlayerIndex = 0;
        currentPlayer = turnOrder[currentPlayerIndex];
        
        EnableAllButtons();
        UpdateGameStateClientRpc();
    }
    
    void OnButtonClicked(int buttonIndex)
    {
        if (!gameStarted) return;
        
        if (IsServer)
        {
            ProcessButtonClick(base.Owner, buttonIndex);
        }
        else
        {
            ProcessButtonClickServerRpc(buttonIndex);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void ProcessButtonClickServerRpc(int buttonIndex, NetworkConnection sender = null)
    {
        ProcessButtonClick(sender, buttonIndex);
    }
    
    void ProcessButtonClick(NetworkConnection sender, int buttonIndex)
    {
        if (!IsServer || !gameStarted) return;
        
        // Check if it's the correct player's turn
        if (currentPlayer.connection != sender && sender != null) return;
        
        // Disable the clicked button
        DisableButtonClientRpc(buttonIndex);
        
        // Disable all buttons temporarily
        DisableAllButtonsClientRpc();
        
        // Move to next player
        NextTurn();
    }
    
    void NextTurn()
    {
        if (!IsServer) return;
        
        currentPlayerIndex = (currentPlayerIndex + 1) % turnOrder.Count;
        currentPlayer = turnOrder[currentPlayerIndex];
        
        // Re-enable buttons for next player (except disabled ones)
        EnableAvailableButtonsClientRpc();
        UpdateGameStateClientRpc();
    }
    
    void EnableAllButtons()
    {
        foreach (var button in gameButtons)
        {
            button.interactable = true;
        }
    }
    
    [ObserversRpc]
    void EnableAvailableButtonsClientRpc()
    {
        foreach (var button in gameButtons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.interactable = true;
            }
        }
    }
    
    [ObserversRpc]
    void DisableAllButtonsClientRpc()
    {
        foreach (var button in gameButtons)
        {
            button.interactable = false;
        }
    }
    
    [ObserversRpc]
    void DisableButtonClientRpc(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < gameButtons.Length)
        {
            gameButtons[buttonIndex].gameObject.SetActive(false);
        }
    }
    
    [ObserversRpc]
    void UpdateGameStateClientRpc()
    {
        if (currentPlayer != null)
        {
            currentTurnText.text = $"Turno de: {currentPlayer.playerName} (Equipo {currentPlayer.teamId})";
            
            string teamColor = currentPlayer.teamId == 1 ? "Azul" : "Rojo";
            gameInfoText.text = $"Equipo {teamColor} - Selecciona un botón";
        }
    }
    
    public void AddPlayer(string playerName, NetworkConnection connection, int teamId, int playerIndex)
    {
        if (!IsServer) return;
        
        GamePlayer newPlayer = new GamePlayer(playerName, connection, teamId, playerIndex);
        
        if (teamId == 1)
        {
            team1Players.Add(newPlayer);
        }
        else if (teamId == 2)
        {
            team2Players.Add(newPlayer);
        }
        
        allPlayers.Add(newPlayer);
        SetupTurnOrder();
    }
    
    public void RemovePlayer(NetworkConnection connection)
    {
        if (!IsServer) return;
        
        allPlayers.RemoveAll(p => p.connection == connection);
        team1Players.RemoveAll(p => p.connection == connection);
        team2Players.RemoveAll(p => p.connection == connection);
        
        SetupTurnOrder();
    }
}