using UnityEngine;

public class LavaKill : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldown = 0.5f;

    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (cooldownTimer > 0f) return;

        if (collision.collider.CompareTag("Player"))
        {
            if (PlayerCompatibilityUtility.TryTakeDamage(collision.collider, damage))
            {
                cooldownTimer = damageCooldown;
            }
        }
    }
}
