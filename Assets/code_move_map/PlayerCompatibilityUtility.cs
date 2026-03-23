using UnityEngine;

public static class PlayerCompatibilityUtility
{
    public static GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static Health GetModernHealth(Component source)
    {
        return source != null ? source.GetComponentInParent<Health>() : null;
    }

    public static Mana GetModernMana(Component source)
    {
        return source != null ? source.GetComponentInParent<Mana>() : null;
    }

    public static PlayerHealth GetLegacyHealth(Component source)
    {
        return source != null ? source.GetComponentInParent<PlayerHealth>() : null;
    }

    public static PlayerMovementController GetModernMovement(Component source)
    {
        return source != null ? source.GetComponentInParent<PlayerMovementController>() : null;
    }

    public static PlayerController GetLegacyController(Component source)
    {
        return source != null ? source.GetComponentInParent<PlayerController>() : null;
    }

    public static bool TryTakeDamage(Component target, int damage)
    {
        Health modernHealth = GetModernHealth(target);
        if (modernHealth != null && !modernHealth.isDead)
        {
            return modernHealth.TakeDamage(damage);
        }

        PlayerHealth legacyHealth = GetLegacyHealth(target);
        if (legacyHealth != null && legacyHealth.currentHealth > 0)
        {
            legacyHealth.TakeDamage(damage);
            return true;
        }

        return false;
    }

    public static bool TryHeal(Component target, int amount)
    {
        Health modernHealth = GetModernHealth(target);
        if (modernHealth != null && !modernHealth.isDead && modernHealth.currentHP < modernHealth.maxHP)
        {
            modernHealth.Heal(amount);
            return true;
        }

        PlayerHealth legacyHealth = GetLegacyHealth(target);
        if (legacyHealth != null && legacyHealth.currentHealth > 0 && legacyHealth.currentHealth < legacyHealth.maxHealth)
        {
            legacyHealth.Heal(amount);
            return true;
        }

        return false;
    }

    public static bool TryRestoreMana(Component target, int amount)
    {
        Mana modernMana = GetModernMana(target);
        if (modernMana != null && modernMana.currentMana < modernMana.maxMana)
        {
            modernMana.RestoreMana(amount);
            return true;
        }

        PlayerHealth legacyHealth = GetLegacyHealth(target);
        if (legacyHealth != null && legacyHealth.currentMana < legacyHealth.maxMana)
        {
            legacyHealth.RestoreMana(amount);
            return true;
        }

        return false;
    }

    public static void ApplyKnockback(Component target, float direction, float speed, float duration)
    {
        PlayerMovementController modernMovement = GetModernMovement(target);
        if (modernMovement != null)
        {
            modernMovement.ApplyKnockback(direction, speed, duration);
            return;
        }

        PlayerController legacyController = GetLegacyController(target);
        if (legacyController != null)
        {
            legacyController.ApplyKnockback(direction, speed, duration);
        }
    }

    public static bool IsDead(GameObject player)
    {
        if (player == null) return false;

        Health modernHealth = player.GetComponent<Health>();
        if (modernHealth != null)
        {
            return modernHealth.isDead;
        }

        PlayerController legacyController = player.GetComponent<PlayerController>();
        if (legacyController != null)
        {
            return legacyController.isDead;
        }

        PlayerHealth legacyHealth = player.GetComponent<PlayerHealth>();
        return legacyHealth != null && legacyHealth.currentHealth <= 0;
    }

    public static bool TryGetHealthValues(GameObject player, out int current, out int max)
    {
        current = 0;
        max = 0;

        if (player == null) return false;

        Health modernHealth = player.GetComponent<Health>();
        if (modernHealth != null)
        {
            current = modernHealth.currentHP;
            max = modernHealth.maxHP;
            return true;
        }

        PlayerHealth legacyHealth = player.GetComponent<PlayerHealth>();
        if (legacyHealth != null)
        {
            current = legacyHealth.currentHealth;
            max = legacyHealth.maxHealth;
            return true;
        }

        return false;
    }

    public static bool TryGetManaValues(GameObject player, out int current, out int max)
    {
        current = 0;
        max = 0;

        if (player == null) return false;

        Mana modernMana = player.GetComponent<Mana>();
        if (modernMana != null)
        {
            current = modernMana.currentMana;
            max = modernMana.maxMana;
            return true;
        }

        PlayerHealth legacyHealth = player.GetComponent<PlayerHealth>();
        if (legacyHealth != null)
        {
            current = legacyHealth.currentMana;
            max = legacyHealth.maxMana;
            return true;
        }

        return false;
    }

    public static void Respawn(GameObject player, Vector3 position)
    {
        if (player == null) return;

        Health modernHealth = player.GetComponent<Health>();
        Mana modernMana = player.GetComponent<Mana>();
        PlayerMovementController modernMovement = player.GetComponent<PlayerMovementController>();
        PlayerCombat modernCombat = player.GetComponent<PlayerCombat>();
        PlayerCombat_BuffHeal modernBuffHealCombat = player.GetComponent<PlayerCombat_BuffHeal>();
        PlayerController legacyController = player.GetComponent<PlayerController>();
        PlayerHealth legacyHealth = player.GetComponent<PlayerHealth>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.GetComponent<Animator>();

        player.transform.position = position;

        if (modernHealth != null)
        {
            modernHealth.isDead = false;
            modernHealth.currentHP = modernHealth.maxHP;
        }

        if (modernMana != null)
        {
            modernMana.currentMana = modernMana.maxMana;
        }

        if (legacyHealth != null)
        {
            legacyHealth.RestoreFullHealthAndMana();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (legacyController != null && legacyHealth != null)
        {
            legacyController.ReviveAt(position);
        }

        if (modernMovement != null)
        {
            modernMovement.enabled = true;
        }

        if (modernCombat != null)
        {
            modernCombat.enabled = true;
        }

        if (modernBuffHealCombat != null)
        {
            modernBuffHealCombat.enabled = true;
        }

        if (legacyController != null && modernMovement != null)
        {
            legacyController.enabled = false;
        }

        if (anim != null)
        {
            anim.ResetTrigger("Death");
            anim.ResetTrigger("Dead");
            anim.Play("Idle");
        }
    }
}
