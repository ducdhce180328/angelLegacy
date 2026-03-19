using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;
    public bool isDead;

    [Header("Block")]
    public bool isBlocking;
    public float blockDamageMultiplier = 0f;

    private Animator anim;
    private bool rewardGiven = false;

    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
    }

    public void SetBlocking(bool value)
    {
        isBlocking = value;
    }

    public bool TakeDamage(int damage)
    {
        if (isDead) return false;

        if (isBlocking)
        {
            damage = Mathf.RoundToInt(damage * blockDamageMultiplier);
        }

        if (damage <= 0) return false;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (currentHP > 0)
        {
            if (anim != null)
                anim.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }

        return true;
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHP = 0;

        GiveRewardToPlayer();

        if (anim != null)
            anim.SetTrigger("Death");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (!CompareTag("Player"))
        {
            Destroy(gameObject, 1.5f);
        }
    }

    void GiveRewardToPlayer()
    {
        if (rewardGiven) return;

        EnemyReward reward = GetComponent<EnemyReward>();
        if (reward == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        stats.AddPoints(reward.rewardPoints);
        rewardGiven = true;
    }
}