using UnityEngine;
using UnityEngine.UI;

public class SetupGameButtons : MonoBehaviour
{
    public static void Execute()
    {
        GameObject gameManager = GameObject.Find("TurnBasedGameManager");
        if (gameManager == null) return;
        
        TurnBasedGameManager manager = gameManager.GetComponent<TurnBasedGameManager>();
        if (manager == null) return;
        
        // Find all game buttons
        Button[] buttons = new Button[6];
        buttons[0] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton1")?.GetComponent<Button>();
        buttons[1] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton2")?.GetComponent<Button>();
        buttons[2] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton3")?.GetComponent<Button>();
        buttons[3] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton4")?.GetComponent<Button>();
        buttons[4] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton5")?.GetComponent<Button>();
        buttons[5] = GameObject.Find("Canvas/GameUI/ButtonContainer/GameButton6")?.GetComponent<Button>();
        
        // Use reflection to set the private field
        var field = typeof(TurnBasedGameManager).GetField("gameButtons", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(manager, buttons);
            Debug.Log("Game buttons array set successfully");
        }
    }
}