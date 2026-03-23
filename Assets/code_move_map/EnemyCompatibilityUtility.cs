using UnityEngine;

public static class EnemyCompatibilityUtility
{
    private const float DefaultKnockbackSpeed = 3.5f;
    private const float DefaultKnockbackDuration = 0.12f;

    public static Component GetDamageReceiver(Component target)
    {
        if (target == null) return null;

        Health modernHealth = target.GetComponentInParent<Health>();
        if (modernHealth != null && !modernHealth.CompareTag("Player"))
        {
            return modernHealth;
        }

        EnemyHealth legacyHealth = target.GetComponentInParent<EnemyHealth>();
        if (legacyHealth != null)
        {
            return legacyHealth;
        }

        return null;
    }

    public static void ApplyDamage(Component receiver, int damage)
    {
        if (receiver is Health modernHealth)
        {
            modernHealth.TakeDamage(damage);
            return;
        }

        if (receiver is EnemyHealth legacyHealth)
        {
            legacyHealth.TakeDamage(damage);
        }
    }

    public static bool ApplyDamageWithKnockback(
        Component receiver,
        int damage,
        Vector3 hitSourcePosition,
        float knockbackSpeed = DefaultKnockbackSpeed,
        float knockbackDuration = DefaultKnockbackDuration)
    {
        if (receiver == null) return false;

        if (receiver is Health modernHealth)
        {
            bool damaged = modernHealth.TakeDamage(damage);
            if (damaged && !modernHealth.isDead)
            {
                ApplyKnockback(receiver, hitSourcePosition, knockbackSpeed, knockbackDuration);
            }

            return damaged;
        }

        if (receiver is EnemyHealth legacyHealth)
        {
            if (legacyHealth.isDead) return false;

            legacyHealth.TakeDamage(damage);

            if (!legacyHealth.isDead)
            {
                legacyHealth.ApplyHitStun(knockbackDuration);
                ApplyKnockback(receiver, hitSourcePosition, knockbackSpeed, knockbackDuration);
            }

            return true;
        }

        return false;
    }

    public static void ApplyKnockback(
        Component receiver,
        Vector3 hitSourcePosition,
        float speed = DefaultKnockbackSpeed,
        float duration = DefaultKnockbackDuration)
    {
        if (receiver == null) return;

        float direction = receiver.transform.position.x >= hitSourcePosition.x ? 1f : -1f;

        EnemyAI modernEnemy = receiver.GetComponentInParent<EnemyAI>();
        if (modernEnemy != null)
        {
            modernEnemy.ApplyKnockback(direction, speed, duration);
            return;
        }

        EnemyPatrol legacyEnemy = receiver.GetComponentInParent<EnemyPatrol>();
        if (legacyEnemy != null)
        {
            legacyEnemy.ApplyKnockback(direction, speed, duration);
            return;
        }

        Rigidbody2D rb = receiver.GetComponentInParent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    public static bool TryDamageEnemy(Component target, int damage)
    {
        Component receiver = GetDamageReceiver(target);
        if (receiver == null) return false;

        ApplyDamage(receiver, damage);
        return true;
    }
}
