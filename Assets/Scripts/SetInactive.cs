using UnityEngine;

public class SetInactive : MonoBehaviour
{
    public static void Execute()
    {
        GameObject teamPanel = GameObject.Find("Canvas/TeamSelectionPanel");
        if (teamPanel != null)
        {
            teamPanel.SetActive(false);
        }
    }
}