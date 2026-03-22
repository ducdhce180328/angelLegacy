using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageCooldown = 1f;

    private float nextDamageTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time < nextDamageTime)
            return;

        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }
}