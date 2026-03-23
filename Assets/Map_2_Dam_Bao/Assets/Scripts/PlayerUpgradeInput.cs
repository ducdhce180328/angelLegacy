using UnityEngine;

public class PlayerUpgradeInput : MonoBehaviour
{
    private PlayerStats stats;
    private LegacyPlayerStats legacyStats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        legacyStats = GetComponent<LegacyPlayerStats>();
    }

    void Update()
    {
        if (stats == null && legacyStats == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (TryUpgradeHP())
                Debug.Log("Da nang cap HP");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (TryUpgradeMana())
                Debug.Log("Da nang cap Mana");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (TryUpgradeAttack())
                Debug.Log("Da nang cap sat thuong danh thuong");
        }
    }

    bool TryUpgradeHP()
    {
        if (stats != null)
        {
            return stats.UpgradeHP();
        }

        return legacyStats != null && legacyStats.UpgradeHP();
    }

    bool TryUpgradeMana()
    {
        if (stats != null)
        {
            return stats.UpgradeMana();
        }

        return legacyStats != null && legacyStats.UpgradeMana();
    }

    bool TryUpgradeAttack()
    {
        if (stats != null)
        {
            return stats.UpgradeNormalAttack();
        }

        return legacyStats != null && legacyStats.UpgradeNormalAttack();
    }
}
