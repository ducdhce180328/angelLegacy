using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;

    private int currentHealth;
    private bool isDead = false;
    private Animator animator;
    private BossController bossController;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        bossController = GetComponent<BossController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current HP: " + currentHealth);

        if (currentHealth > 0)
        {
            if (bossController != null)
            {
                bossController.OnTakeHit();
            }

            if (animator != null)
            {
                animator.ResetTrigger("TakeHit");
                animator.SetTrigger("TakeHit");
            }
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died.");

        if (bossController != null)
        {
            bossController.OnDeath();
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Gọi hàm này bằng Animation Event ở cuối animation Death
    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}