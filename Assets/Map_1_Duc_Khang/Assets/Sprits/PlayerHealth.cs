using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Mana")]
    public int maxMana = 100;
    public int currentMana;

    [Header("UI - Health")]
    public Image healthBarImage;
    public TextMeshProUGUI healthText;

    [Header("UI - Mana")]
    public Image manaBarImage;
    public TextMeshProUGUI manaText;

    [Header("Health Sprites")]
    public Sprite[] healthSprites;      // greenMeter01 -> greenMeter10
    public Sprite emptyHealthSprite;

    [Header("Mana Sprites")]
    public Sprite[] manaSprites;        // blueMeter01 -> blueMeter10
    public Sprite emptyManaSprite;

    private PlayerController playerController;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        playerController = GetComponent<PlayerController>();

        UpdateHealthUI();
        UpdateManaUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth > 0)
        {
            if (playerController != null)
            {
                playerController.TakeHit();
            }
        }
        else
        {
            Die();
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;

        if (currentMana < 0)
            currentMana = 0;

        UpdateManaUI();
        return true;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;

        if (currentMana > maxMana)
            currentMana = maxMana;

        UpdateManaUI();
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void AddHealth(int amount)
    {
        Heal(amount);
    }

    public void AddMana(int amount)
    {
        RestoreMana(amount);
    }

    public void RestoreFullHealthAndMana()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        UpdateHealthUI();
        UpdateManaUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;

        if (healthBarImage != null)
        {
            if (currentHealth <= 0)
            {
                healthBarImage.sprite = emptyHealthSprite;
                return;
            }

            if (healthSprites != null && healthSprites.Length > 0)
            {
                int level = Mathf.CeilToInt((float)currentHealth / maxHealth * 10f);
                level = Mathf.Clamp(level, 1, 10);

                int spriteIndex = Mathf.Clamp(level - 1, 0, healthSprites.Length - 1);
                healthBarImage.sprite = healthSprites[spriteIndex];
            }
        }
    }

    void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = currentMana + " / " + maxMana;

        if (manaBarImage != null)
        {
            if (currentMana <= 0)
            {
                manaBarImage.sprite = emptyManaSprite;
                return;
            }

            if (manaSprites != null && manaSprites.Length > 0)
            {
                int level = Mathf.CeilToInt((float)currentMana / maxMana * 10f);
                level = Mathf.Clamp(level, 1, 10);

                int spriteIndex = Mathf.Clamp(level - 1, 0, manaSprites.Length - 1);
                manaBarImage.sprite = manaSprites[spriteIndex];
            }
        }
    }

    void Die()
    {
        if (playerController != null)
        {
            playerController.Die();
        }
    }
}