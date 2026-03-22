using UnityEngine;
using System.Collections;

public class BearHealth : MonoBehaviour
{
    [Header("HP")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Damage Effect")]
    public float damageDuration = 0.3f;
    public bool isDead = false;

    private BearAI bearAI;

    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        bearAI = GetComponent<BearAI>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        StartCoroutine(TakeDamageOverTime(damage));
    }

    private IEnumerator TakeDamageOverTime(float damage)
    {
        float elapsed = 0f;
        float startHealth = currentHealth;
        float targetHealth = Mathf.Max(0, currentHealth - damage);

        while (elapsed < damageDuration)
        {
            elapsed += Time.deltaTime;
            currentHealth = Mathf.Lerp(startHealth, targetHealth, elapsed / damageDuration);
            yield return null;
        }

        currentHealth = targetHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (bearAI != null)
            bearAI.Die();
    }
}