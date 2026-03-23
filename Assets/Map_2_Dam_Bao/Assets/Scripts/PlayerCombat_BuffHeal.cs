using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat_BuffHeal : MonoBehaviour
{
    [Header("Attack Point")]
    public Transform attackPoint;
    public float attackRange = 0.7f;
    public LayerMask enemyLayer;

    [Header("Normal Attack")]
    public int normalAttackDamage = 15;
    public float attack1Cooldown = 0.5f;

    [Header("Skill K - Damage Buff")]
    public int buffExtraDamage = 15;
    public float buffDuration = 5f;
    public float buffCooldown = 8f;

    [Header("Skill L - Heal")]
    public int healAmount = 30;
    public int healManaCost = 20;
    public float healCooldown = 10f;

    [Header("Gamepad Buttons")]
    public KeyCode gamepadNormalAttackButton = KeyCode.JoystickButton2; // PS Square
    public KeyCode gamepadBuffButton = KeyCode.JoystickButton3; // PS Triangle
    public KeyCode gamepadHealButton = KeyCode.JoystickButton1; // PS Circle

    private float attack1Timer;
    private float buffTimer;
    private float healTimer;

    private bool isBuffActive = false;

    private Animator anim;
    private Mana mana;
    private Health health;

    void Start()
    {
        anim = GetComponent<Animator>();
        mana = GetComponent<Mana>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (health != null && health.isDead) return;

        attack1Timer -= Time.deltaTime;
        buffTimer -= Time.deltaTime;
        healTimer -= Time.deltaTime;

        if (NormalAttackPressed() && attack1Timer <= 0f)
        {
            attack1Timer = attack1Cooldown;

            if (anim != null)
                anim.SetTrigger("Attack1");

            DoNormalAttack();
        }

        if (BuffPressed() && buffTimer <= 0f)
        {
            buffTimer = buffCooldown;

            if (anim != null)
                anim.SetTrigger("Attack2");

            StartCoroutine(DamageBuffRoutine());
        }

        if (HealPressed() && healTimer <= 0f)
        {
            if (mana != null && mana.UseMana(healManaCost))
            {
                healTimer = healCooldown;

                if (anim != null)
                    anim.SetTrigger("Attack3");

                if (health != null)
                    health.Heal(healAmount);
            }
            else
            {
                Debug.Log("Khong du mana de hoi mau");
            }
        }
    }

    bool NormalAttackPressed()
    {
        return Input.GetKeyDown(KeyCode.J)
            || Input.GetKeyDown(gamepadNormalAttackButton);
    }

    bool BuffPressed()
    {
        return Input.GetKeyDown(KeyCode.K)
            || Input.GetKeyDown(gamepadBuffButton);
    }

    bool HealPressed()
    {
        return Input.GetKeyDown(KeyCode.L)
            || Input.GetKeyDown(gamepadHealButton);
    }

    void DoNormalAttack()
    {
        int finalDamage = normalAttackDamage;

        if (isBuffActive)
            finalDamage += buffExtraDamage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        HashSet<Component> damagedTargets = new HashSet<Component>();

        foreach (Collider2D hit in hits)
        {
            Component receiver = EnemyCompatibilityUtility.GetDamageReceiver(hit);
            if (receiver == null || damagedTargets.Contains(receiver)) continue;

            damagedTargets.Add(receiver);
            EnemyCompatibilityUtility.ApplyDamageWithKnockback(receiver, finalDamage, attackPoint.position);
        }
    }

    IEnumerator DamageBuffRoutine()
    {
        isBuffActive = true;
        yield return new WaitForSeconds(buffDuration);
        isBuffActive = false;
    }

    public void UpgradeAttackRange(float amount)
    {
        attackRange += amount;
    }

    public void UpgradeNormalAttack(int amount)
    {
        normalAttackDamage += amount;
    }

    public void UpgradeBuffExtraDamage(int amount)
    {
        buffExtraDamage += amount;
    }

    public void UpgradeHealAmount(int amount)
    {
        healAmount += amount;
    }

    public void ReduceHealManaCost(int amount)
    {
        healManaCost -= amount;
        if (healManaCost < 1)
            healManaCost = 1;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
