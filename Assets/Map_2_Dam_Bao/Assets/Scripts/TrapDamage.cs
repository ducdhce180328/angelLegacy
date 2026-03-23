using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damage = 15;
    public float damageCooldown = 0.5f;

    private float cooldownTimer;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (cooldownTimer > 0f) return;

        if (other.CompareTag("Player"))
        {
            if (PlayerCompatibilityUtility.TryTakeDamage(other, damage))
            {
                cooldownTimer = damageCooldown;
            }
        }
    }
}
