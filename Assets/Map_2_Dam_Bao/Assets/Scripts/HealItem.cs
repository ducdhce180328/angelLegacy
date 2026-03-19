using UnityEngine;

public class HealItem : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Health playerHealth = other.GetComponentInParent<Health>();
        if (playerHealth != null && !playerHealth.isDead)
        {
            if (playerHealth.currentHP < playerHealth.maxHP)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}