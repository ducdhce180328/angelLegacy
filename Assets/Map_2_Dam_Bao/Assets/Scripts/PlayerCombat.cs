using UnityEngine;
using System.Collections.Generic;

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

    [Header("Gamepad Buttons")]
    public KeyCode gamepadNormalAttackButton = KeyCode.JoystickButton2; // PS Square
    public KeyCode gamepadBlockButton = KeyCode.JoystickButton3; // PS Triangle
    public KeyCode gamepadSkillButton = KeyCode.JoystickButton1; // PS Circle

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
        if (NormalAttackPressed() && attack1Timer <= 0)
        {
            attack1Timer = attack1Cooldown;

            if (anim != null)
                anim.SetTrigger("Attack1");

            DoAttack(normalAttackDamage);
        }

        // L = skill tốn mana
        if (SkillPressed() && attack3Timer <= 0)
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
        if (BlockPressed())
        {
            isBlocking = true;

            if (anim != null)
            {
                anim.SetBool("Block", true);
                anim.SetBool("IdleBlock", true);
            }
        }

        if (BlockReleased())
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

    bool NormalAttackPressed()
    {
        return Input.GetKeyDown(KeyCode.J)
            || Input.GetKeyDown(gamepadNormalAttackButton);
    }

    bool SkillPressed()
    {
        return Input.GetKeyDown(KeyCode.L)
            || Input.GetKeyDown(gamepadSkillButton);
    }

    bool BlockPressed()
    {
        return Input.GetKeyDown(KeyCode.K)
            || Input.GetKeyDown(gamepadBlockButton);
    }

    bool BlockReleased()
    {
        return Input.GetKeyUp(KeyCode.K)
            || Input.GetKeyUp(gamepadBlockButton);
    }

    void DoAttack(int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        HashSet<Component> damagedTargets = new HashSet<Component>();

        foreach (Collider2D hit in hits)
        {
            Component receiver = EnemyCompatibilityUtility.GetDamageReceiver(hit);
            if (receiver == null || damagedTargets.Contains(receiver)) continue;

            damagedTargets.Add(receiver);
            EnemyCompatibilityUtility.ApplyDamageWithKnockback(receiver, damage, attackPoint.position);
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
