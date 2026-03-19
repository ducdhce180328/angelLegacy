using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Point")]
    public Transform attackPoint;
    public float attackRange = 0.7f;
    public LayerMask enemyLayer;

    [Header("Damage")]
    public int normalAttackDamage = 15;
    public int skillManaDamage = 40;

    [Header("Cooldown")]
    public float attack1Cooldown = 0.5f;
    public float attack3Cooldown = 3f;

    [Header("Mana")]
    public int attack3ManaCost = 20;

    private float attack1Timer;
    private float attack3Timer;

    private Animator anim;
    private Mana mana;
    private Health health;

    private bool isBlocking;

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
        attack3Timer -= Time.deltaTime;

        HandleBlock();

        if (isBlocking) return;

        // J = đánh thường
        if (Input.GetKeyDown(KeyCode.J) && attack1Timer <= 0)
        {
            attack1Timer = attack1Cooldown;

            if (anim != null)
                anim.SetTrigger("Attack1");

            DoAttack(normalAttackDamage);
        }

        // L = skill tốn mana
        if (Input.GetKeyDown(KeyCode.L) && attack3Timer <= 0)
        {
            if (mana != null && mana.UseMana(attack3ManaCost))
            {
                attack3Timer = attack3Cooldown;

                if (anim != null)
                    anim.SetTrigger("Attack3");

                DoAttack(skillManaDamage);
            }
            else
            {
                Debug.Log("Khong du mana");
            }
        }
    }

    void HandleBlock()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isBlocking = true;

            if (anim != null)
            {
                anim.SetBool("Block", true);
                anim.SetBool("IdleBlock", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            isBlocking = false;

            if (anim != null)
            {
                anim.SetBool("Block", false);
                anim.SetBool("IdleBlock", false);
            }
        }

        if (health != null)
        {
            health.SetBlocking(isBlocking);
        }
    }

    void DoAttack(int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void UpgradeSkillDamage(int amount)
{
    skillManaDamage += amount;
}

public void UpgradeAttackRange(float amount)
{
    attackRange += amount;
}

public void ReduceSkillManaCost(int amount)
{
    attack3ManaCost -= amount;
    if (attack3ManaCost < 1)
        attack3ManaCost = 1;
}
}