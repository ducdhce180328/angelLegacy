using UnityEngine;

public class LegacyPlayerStats : MonoBehaviour
{
    [Header("Points")]
    public int upgradePoints = 0;

    [Header("Upgrade Levels")]
    public int hpLevel = 0;
    public int manaLevel = 0;
    public int attackLevel = 0;

    [Header("Upgrade Values")]
    public int hpIncreasePerLevel = 20;
    public int manaIncreasePerLevel = 20;
    public int normalAttackIncreasePerLevel = 5;

    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private UpgradeEffectSpawner upgradeEffectSpawner;

    private void Awake()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (upgradeEffectSpawner == null)
        {
            upgradeEffectSpawner = GetComponent<UpgradeEffectSpawner>();
        }
    }

    public void AddPoints(int amount)
    {
        upgradePoints += amount;
    }

    public bool UpgradeHP()
    {
        CacheReferences();

        if (upgradePoints <= 0 || playerHealth == null) return false;

        upgradePoints--;
        hpLevel++;
        playerHealth.maxHealth += hpIncreasePerLevel;
        playerHealth.currentHealth = Mathf.Clamp(
            playerHealth.currentHealth + hpIncreasePerLevel,
            0,
            playerHealth.maxHealth
        );

        PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.HP);
        return true;
    }

    public bool UpgradeMana()
    {
        CacheReferences();

        if (upgradePoints <= 0 || playerHealth == null) return false;

        upgradePoints--;
        manaLevel++;
        playerHealth.maxMana += manaIncreasePerLevel;
        playerHealth.currentMana = Mathf.Clamp(
            playerHealth.currentMana + manaIncreasePerLevel,
            0,
            playerHealth.maxMana
        );

        PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.Mana);
        return true;
    }

    public bool UpgradeNormalAttack()
    {
        CacheReferences();

        if (upgradePoints <= 0 || playerController == null) return false;

        upgradePoints--;
        attackLevel++;
        playerController.normalAttackDamage += normalAttackIncreasePerLevel;

        PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.Attack);
        return true;
    }

    private void PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType effectType)
    {
        if (upgradeEffectSpawner != null)
        {
            upgradeEffectSpawner.PlayEffect(effectType);
        }
    }
}
