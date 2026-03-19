using UnityEngine;
using System.Collections;

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

        if (Input.GetKeyDown(KeyCode.J) && attack1Timer <= 0f)
        {
            attack1Timer = attack1Cooldown;

            if (anim != null)
                anim.SetTrigger("Attack1");

            DoNormalAttack();
        }

        if (Input.GetKeyDown(KeyCode.K) && buffTimer <= 0f)
        {
            buffTimer = buffCooldown;

            if (anim != null)
                anim.SetTrigger("Attack2");

            StartCoroutine(DamageBuffRoutine());
        }

        if (Input.GetKeyDown(KeyCode.L) && healTimer <= 0f)
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

    void DoNormalAttack()
    {
        int finalDamage = normalAttackDamage;

        if (isBuffActive)
            finalDamage += buffExtraDamage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(finalDamage);
            }
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