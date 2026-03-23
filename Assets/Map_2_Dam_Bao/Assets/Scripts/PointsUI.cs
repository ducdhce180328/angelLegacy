using UnityEngine;
using TMPro;

public class PointsUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public LegacyPlayerStats legacyPlayerStats;
    public TMP_Text pointsText;

    void Update()
    {
        FindPlayerIfNeeded();

        if (pointsText == null) return;

        if (playerStats != null)
        {
            pointsText.text = "Points: " + playerStats.upgradePoints;
            return;
        }

        if (legacyPlayerStats != null)
        {
            pointsText.text = "Points: " + legacyPlayerStats.upgradePoints;
        }
    }

    void FindPlayerIfNeeded()
    {
        if (playerStats != null || legacyPlayerStats != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        playerStats = player.GetComponent<PlayerStats>();
        legacyPlayerStats = player.GetComponent<LegacyPlayerStats>();
    }
}
