using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class LocalGamePlayer
{
    public string playerName;
    public int teamId;
    public int playerIndex; // 0 or 1 within the team
    
    public LocalGamePlayer(string name, int team, int index)
    {
        playerName = name;
        teamId = team;
        playerIndex = index;
    }
}

public class LocalTurnBasedGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Text currentTurnText;
    public Text gameInfoText;
    public Button[] gameButtons; // Interactive buttons for the game
    public GameObject gameUI;
    
    [Header("Game Settings")]
    public int totalButtons = 6;
    
    private List<LocalGamePlayer> team1Players = new List<LocalGamePlayer>();
    private List<LocalGamePlayer> team2Players = new List<LocalGamePlayer>();
    private List<LocalGamePlayer> allPlayers = new List<LocalGamePlayer>();
    
    private int currentPlayerIndex = 0;
    private LocalGamePlayer currentPlayer;
    private bool gameStarted = false;
    
    // Turn order: Team1Player1 -> Team2Player1 -> Team1Player2 -> Team2Player2 -> repeat
    private List<LocalGamePlayer> turnOrder = new List<LocalGamePlayer>();
    
    void Start()
    {
        InitializeGame();
        SetupGameButtons();
    }
    
    void InitializeGame()
    {
        // Create test players
        team1Players.Add(new LocalGamePlayer("Player1", 1, 0));
        team1Players.Add(new LocalGamePlayer("Player2", 1, 1));
        team2Players.Add(new LocalGamePlayer("Player3", 2, 0));
        team2Players.Add(new LocalGamePlayer("Player4", 2, 1));
        
        allPlayers.AddRange(team1Players);
        allPlayers.AddRange(team2Players);
        
        SetupTurnOrder();
        StartGameRound();
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
        // Find buttons if not assigned
        if (gameButtons == null || gameButtons.Length == 0)
        {
            gameButtons = new Button[totalButtons];
            for (int i = 0; i < totalButtons; i++)
            {
                GameObject buttonObj = GameObject.Find($"Canvas/GameUI/ButtonContainer/GameButton{i + 1}");
                if (buttonObj != null)
                {
                    gameButtons[i] = buttonObj.GetComponent<Button>();
                }
            }
        }
        
        // Setup button listeners
        for (int i = 0; i < gameButtons.Length; i++)
        {
            if (gameButtons[i] != null)
            {
                int buttonIndex = i; // Capture for closure
                gameButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex));
            }
        }
    }
    
    void StartGameRound()
    {
        gameStarted = true;
        currentPlayerIndex = 0;
        currentPlayer = turnOrder[currentPlayerIndex];
        
        EnableAllButtons();
        UpdateGameState();
    }
    
    void OnButtonClicked(int buttonIndex)
    {
        if (!gameStarted) return;
        
        // Disable the clicked button
        if (buttonIndex >= 0 && buttonIndex < gameButtons.Length && gameButtons[buttonIndex] != null)
        {
            gameButtons[buttonIndex].gameObject.SetActive(false);
        }
        
        // Disable all buttons temporarily
        DisableAllButtons();
        
        // Move to next player after a short delay
        Invoke("NextTurn", 1f);
    }
    
    void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % turnOrder.Count;
        currentPlayer = turnOrder[currentPlayerIndex];
        
        // Re-enable buttons for next player (except disabled ones)
        EnableAvailableButtons();
        UpdateGameState();
    }
    
    void EnableAllButtons()
    {
        foreach (var button in gameButtons)
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
    
    void EnableAvailableButtons()
    {
        foreach (var button in gameButtons)
        {
            if (button != null && button.gameObject.activeInHierarchy)
            {
                button.interactable = true;
            }
        }
    }
    
    void DisableAllButtons()
    {
        foreach (var button in gameButtons)
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }
    
    void UpdateGameState()
    {
        if (currentPlayer != null)
        {
            currentTurnText.text = $"Turno de: {currentPlayer.playerName} (Equipo {currentPlayer.teamId})";
            
            string teamColor = currentPlayer.teamId == 1 ? "Azul" : "Rojo";
            gameInfoText.text = $"Equipo {teamColor} - Selecciona un botón";
            
            // Change text color based on team
            if (currentPlayer.teamId == 1)
            {
                gameInfoText.color = new Color(0.2f, 0.6f, 1f, 1f); // Blue
            }
            else
            {
                gameInfoText.color = new Color(1f, 0.3f, 0.3f, 1f); // Red
            }
        }
        
        // Check if game is over (all buttons clicked)
        int activeButtons = 0;
        foreach (var button in gameButtons)
        {
            if (button != null && button.gameObject.activeInHierarchy)
            {
                activeButtons++;
            }
        }
        
        if (activeButtons == 0)
        {
            EndGame();
        }
    }
    
    void EndGame()
    {
        gameStarted = false;
        currentTurnText.text = "¡Juego Terminado!";
        gameInfoText.text = "Todos los botones han sido seleccionados";
        gameInfoText.color = Color.yellow;
        
        // Option to restart
        Invoke("RestartGame", 3f);
    }
    
    void RestartGame()
    {
        // Reactivate all buttons
        foreach (var button in gameButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(true);
                button.interactable = true;
            }
        }
        
        // Restart the game
        StartGameRound();
    }
}