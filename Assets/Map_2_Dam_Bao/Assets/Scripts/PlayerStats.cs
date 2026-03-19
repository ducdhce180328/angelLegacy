using UnityEngine;

public class PlayerStats : MonoBehaviour
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

    private Health health;
    private Mana mana;
    private PlayerCombat combat;
    private PlayerCombat_BuffHeal buffHealCombat;
    private UpgradeEffectSpawner upgradeEffectSpawner;
    

   void Start()
{
    health = GetComponent<Health>();
    mana = GetComponent<Mana>();
    combat = GetComponent<PlayerCombat>();
    buffHealCombat = GetComponent<PlayerCombat_BuffHeal>();
    upgradeEffectSpawner = GetComponent<UpgradeEffectSpawner>();
}

    public void AddPoints(int amount)
    {
        upgradePoints += amount;
    }

    public bool UpgradeHP()
{
    if (upgradePoints <= 0 || health == null) return false;

    upgradePoints--;
    hpLevel++;

    health.maxHP += hpIncreasePerLevel;
    health.currentHP += hpIncreasePerLevel;

    PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.HP);
    return true;
}

    public bool UpgradeMana()
{
    if (upgradePoints <= 0 || mana == null) return false;

    upgradePoints--;
    manaLevel++;

    mana.maxMana += manaIncreasePerLevel;
    mana.currentMana += manaIncreasePerLevel;

    PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.Mana);
    return true;
}

    public bool UpgradeNormalAttack()
{
    if (upgradePoints <= 0) return false;

    upgradePoints--;
    attackLevel++;

    if (combat != null)
    {
        combat.normalAttackDamage += normalAttackIncreasePerLevel;
        PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.Attack);
        return true;
    }

    if (buffHealCombat != null)
    {
        buffHealCombat.UpgradeNormalAttack(normalAttackIncreasePerLevel);
        PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType.Attack);
        return true;
    }

    return false;
}
void PlayUpgradeEffect(UpgradeEffectSpawner.UpgradeEffectType effectType)
{
    if (upgradeEffectSpawner != null)
    {
        upgradeEffectSpawner.PlayEffect(effectType);
    }
}
}