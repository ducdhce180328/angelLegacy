using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Mana")]
    [SerializeField] private int maxMana = 50;
    [SerializeField] private int currentMana = 50;

    [Header("Mana Regeneration")]
    [SerializeField] private bool enableManaRegen = true;
    [SerializeField] private float manaRegenInterval = 1f;
    [SerializeField] private int manaRegenAmount = 2;

    [Header("Life Steal")]
    [SerializeField] private int lifeStealAmount = 1;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxMana => maxMana;
    public int CurrentMana => currentMana;

    private PlayerUIManager ui;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead;
    private float manaRegenTimer;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        ui = FindFirstObjectByType<PlayerUIManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        UpdateHealthUI();
        UpdateManaUI();
    }

    private void Update()
    {
        HandleManaRegen();

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(10);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            UseMana(5);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            RestoreMana(5);
        }
    }

    private void HandleManaRegen()
    {
        if (!enableManaRegen)
            return;

        if (isDead)
            return;

        if (currentMana >= maxMana)
        {
            manaRegenTimer = 0f;
            return;
        }

        manaRegenTimer += Time.deltaTime;

        if (manaRegenTimer >= manaRegenInterval)
        {
            manaRegenTimer -= manaRegenInterval;
            RestoreMana(manaRegenAmount);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayGetHit();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    public void HealOnHit()
    {
        if (isDead)
            return;

        if (lifeStealAmount <= 0)
            return;

        Heal(lifeStealAmount);
    }

    public bool UseMana(int amount)
    {
        if (isDead)
            return false;

        if (currentMana < amount)
            return false;

        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
        return true;
    }

    public void RestoreMana(int amount)
    {
        if (isDead)
            return;

        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateManaUI();
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void PlayGetHit()
    {
        if (animator == null || isDead)
            return;

        animator.ResetTrigger("GetHit");
        animator.SetTrigger("GetHit");
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Debug.Log("Player Dead");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Skill1");
            animator.ResetTrigger("GetHit");
            animator.ResetTrigger("DeathTrigger");
            animator.SetTrigger("DeathTrigger");
        }
    }

    private void UpdateHealthUI()
    {
        if (ui == null)
        {
            ui = FindFirstObjectByType<PlayerUIManager>();
        }

        if (ui != null)
        {
            ui.UpdateHealth(currentHealth, maxHealth);
        }
    }

    private void UpdateManaUI()
    {
        if (ui == null)
        {
            ui = FindFirstObjectByType<PlayerUIManager>();
        }

        if (ui != null)
        {
            ui.UpdateMana(currentMana, maxMana);
        }
    }
}