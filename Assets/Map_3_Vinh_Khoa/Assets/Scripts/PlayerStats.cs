using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Mana")]
    [SerializeField] private int maxMana = 50;
    [SerializeField] private int currentMana = 50;

    [Header("Respawn")]
    [SerializeField] private Transform startCheckpoint;
    [SerializeField] private float respawnDelay = 1.2f;

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
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private bool isDead;
    private float manaRegenTimer;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        ui = FindFirstObjectByType<PlayerUIManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();

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
        if (isDead)
            return;

        if (playerCombat != null)
        {
            playerCombat.OnTakeHitInterrupt();
        }

        if (animator == null)
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

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Skill1");
            animator.ResetTrigger("GetHit");
            animator.ResetTrigger("DeathTrigger");
            animator.SetTrigger("DeathTrigger");
        }

        StartCoroutine(RespawnCoroutine());
    }


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (startCheckpoint != null)
        {
            transform.position = startCheckpoint.position;
        }

        currentHealth = maxHealth;
        currentMana = maxMana;
        isDead = false;
        manaRegenTimer = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 3f;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerCombat != null)
        {
            playerCombat.enabled = true;
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        UpdateHealthUI();
        UpdateManaUI();
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