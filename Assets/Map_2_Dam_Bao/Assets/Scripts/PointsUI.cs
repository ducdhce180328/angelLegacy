using UnityEngine;
using TMPro;

public class PointsUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TMP_Text pointsText;

    void Update()
    {
        FindPlayerIfNeeded();

        if (playerStats == null || pointsText == null) return;

        pointsText.text = "Points: " + playerStats.upgradePoints;
    }

    void FindPlayerIfNeeded()
    {
        if (playerStats != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        playerStats = player.GetComponent<PlayerStats>();
    }
}