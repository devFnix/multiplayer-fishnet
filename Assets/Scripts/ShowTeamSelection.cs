using UnityEngine;

public class ShowTeamSelection : MonoBehaviour
{
    public static void Execute()
    {
        GameObject teamPanel = GameObject.Find("Canvas/TeamSelectionPanel");
        if (teamPanel != null)
        {
            teamPanel.SetActive(true);
        }
        
        GameObject mainMenu = GameObject.Find("Canvas/MainMenuPanel");
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
        }
        
        GameObject roomPanel = GameObject.Find("Canvas/RoomPanel");
        if (roomPanel != null)
        {
            roomPanel.SetActive(false);
        }
        
        Debug.Log("Team selection panel shown for testing");
    }
}