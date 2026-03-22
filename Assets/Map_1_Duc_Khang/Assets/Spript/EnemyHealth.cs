using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Combat")]
    public int damageTakenFromPlayer = 20;
    public int damageToPlayer = 5;
    public int scoreValue = 1;

    [Header("UI")]
    public Image healthBarImage;

    [Header("Health Sprites")]
    public Sprite[] healthSprites;
    public Sprite emptyHealthSprite;

    [Header("Gate Unlock On Death")]
    public GameObject gateToOpenWhenDead;
    [TextArea]
    public string openGateMessage = "Đã mở đường đi tiếp!";

    private Animator anim;
    public bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth > 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("Hurt");
            }
        }
        else
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarImage == null) return;

        if (currentHealth <= 0)
        {
            healthBarImage.sprite = emptyHealthSprite;
            return;
        }

        if (healthSprites == null || healthSprites.Length == 0) return;

        int level = Mathf.CeilToInt((float)currentHealth / maxHealth * 10f);
        level = Mathf.Clamp(level, 1, 10);

        int spriteIndex = Mathf.Clamp(level - 1, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[spriteIndex];
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("Dead");
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }

        if (gateToOpenWhenDead != null)
        {
            gateToOpenWhenDead.SetActive(false);
        }

        if (UIManager.instance != null && !string.IsNullOrEmpty(openGateMessage) && gateToOpenWhenDead != null)
        {
            UIManager.instance.ShowUnlockMessage(openGateMessage);
        }

        Destroy(gameObject, 0.5f);
    }
}