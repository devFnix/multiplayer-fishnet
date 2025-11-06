using UnityEngine;
using UnityEngine.UI;

public class TeamTestHelper : MonoBehaviour
{
    [Header("Test Buttons")]
    public Button simulatePlayer1JoinTeam1;
    public Button simulatePlayer2JoinTeam1;
    public Button simulatePlayer3JoinTeam2;
    public Button simulatePlayer4JoinTeam2;
    
    private LocalTeamManager teamManager;
    
    void Start()
    {
        teamManager = FindObjectOfType<LocalTeamManager>();
        
        if (simulatePlayer1JoinTeam1 != null)
            simulatePlayer1JoinTeam1.onClick.AddListener(() => SimulateJoin("Player1", 1));
        if (simulatePlayer2JoinTeam1 != null)
            simulatePlayer2JoinTeam1.onClick.AddListener(() => SimulateJoin("Player2", 1));
        if (simulatePlayer3JoinTeam2 != null)
            simulatePlayer3JoinTeam2.onClick.AddListener(() => SimulateJoin("Player3", 2));
        if (simulatePlayer4JoinTeam2 != null)
            simulatePlayer4JoinTeam2.onClick.AddListener(() => SimulateJoin("Player4", 2));
    }
    
    void SimulateJoin(string playerName, int teamId)
    {
        if (teamManager != null)
        {
            teamManager.SimulatePlayerJoinTeam(playerName, teamId);
        }
    }
    
    // Keyboard shortcuts for testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SimulateJoin("Player1", 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SimulateJoin("Player2", 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SimulateJoin("Player3", 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SimulateJoin("Player4", 2);
        }
    }
}