using UnityEngine;

public class PlayerUpgradeInput : MonoBehaviour
{
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (stats == null) return;

        // 1 = tăng HP
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (stats.UpgradeHP())
                Debug.Log("Da nang cap HP");
        }

        // 2 = tăng Mana
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (stats.UpgradeMana())
                Debug.Log("Da nang cap Mana");
        }

        // 3 = tăng sát thương đánh thường
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (stats.UpgradeNormalAttack())
                Debug.Log("Da nang cap sat thuong danh thuong");
        }
    }
}