using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isHurt = false;
    public bool isDead = false;

    [Header("Normal Attack J")]
    public Transform attackPoint;
    public float attackRadius = 0.6f;
    public LayerMask enemyLayer;
    public int normalAttackDamage = 10;
    public float normalAttackDuration = 0.35f;

    [Header("Skill Attack K")]
    public int skillDamage = 20;
    public int skillManaCost = 5;
    public Transform skillFirePoint;
    public GameObject skillProjectilePrefab;
    public float skillProjectileSpeed = 8f;

    [Header("Unlock Skill L")]
    public bool hasUnlockedSkillL = false;
    public float skillLCooldown = 10f;
    public int skillLDamage = 30;
    public Transform skillLFirePoint;
    public GameObject skillLProjectilePrefab;
    public float skillLProjectileSpeed = 8f;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private bool isGrounded;
    private bool isAttacking;
    private bool isSkillLOnCooldown;
    private Vector3 originalScale;
    private Vector3 respawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;
        respawnPosition = transform.position;
        isSkillLOnCooldown = false;

        hasUnlockedSkillL = false;
    }

    private void Update()
    {
        CheckGround();

        if (isDead)
        {
            moveInput = 0f;
            UpdateAnimator();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        if (!isAttacking && !isHurt)
        {
            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                NormalAttack();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                SkillAttack();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                UseSkillL();
            }
        }
        else
        {
            moveInput = 0f;
        }

        Flip();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isAttacking || isHurt)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ) != null;
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("IsDead", isDead);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void Flip()
    {
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void NormalAttack()
    {
        if (isDead || isAttacking || isHurt) return;
        StartCoroutine(NormalAttackRoutine());
    }

    private IEnumerator NormalAttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("NormalAttack");
        }

        yield return new WaitForSeconds(normalAttackDuration);
        isAttacking = false;
    }

    public void UnlockSkillL()
    {
        hasUnlockedSkillL = true;
    }

    void UseSkillL()
    {
        if (!hasUnlockedSkillL) return;
        if (isDead || isAttacking || isHurt || isSkillLOnCooldown) return;

        StartCoroutine(SkillLRoutine());
    }

    private IEnumerator SkillLRoutine()
    {
        isAttacking = true;
        isSkillLOnCooldown = true;
        StartCoroutine(SkillLCooldownRoutine());

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("SkillL");
        }

        yield return new WaitForSeconds(0.15f);

        FireSkillLProjectile();

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    private IEnumerator SkillLCooldownRoutine()
    {
        yield return new WaitForSeconds(skillLCooldown);
        isSkillLOnCooldown = false;
    }

    void SkillAttack()
    {
        if (isDead || isAttacking || isHurt) return;

        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        bool usedMana = playerHealth.UseMana(skillManaCost);
        if (!usedMana) return;

        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    public void FireSkillProjectile()
    {
        if (skillProjectilePrefab == null || skillFirePoint == null) return;

        GameObject projectileObj = Instantiate(
            skillProjectilePrefab,
            skillFirePoint.position,
            Quaternion.identity
        );

        SkillKProjectile projectile = projectileObj.GetComponent<SkillKProjectile>();

        float direction = transform.localScale.x >= 0 ? 1f : -1f;

        if (projectile != null)
        {
            projectile.Init(direction, skillDamage, skillProjectileSpeed);
        }
    }

    public void FireSkillLProjectile()
    {
        if (skillLProjectilePrefab == null || skillLFirePoint == null) return;

        GameObject projectileObj = Instantiate(
            skillLProjectilePrefab,
            skillLFirePoint.position + new Vector3(0f, 0.2f, 0f),
            Quaternion.identity
        );

        SkillLProjectile projectile = projectileObj.GetComponent<SkillLProjectile>();

        float direction = transform.localScale.x >= 0 ? 1f : -1f;

        if (projectile != null)
        {
            projectile.Init(direction, skillLDamage, skillLProjectileSpeed);
        }
    }

    public void DealNormalDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null && !damagedEnemies.Contains(enemyHealth))
            {
                damagedEnemies.Add(enemyHealth);
                enemyHealth.TakeDamage(normalAttackDamage);
            }
        }
    }

    public void DealSkillDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null && !damagedEnemies.Contains(enemyHealth))
            {
                damagedEnemies.Add(enemyHealth);
                enemyHealth.TakeDamage(skillDamage);
            }
        }
    }

    public void DealDamage()
    {
        DealSkillDamage();
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeHit()
    {
        if (isDead) return;

        isHurt = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("Hurt");
        }
    }

    public void EndHurt()
    {
        isHurt = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        isHurt = false;
        moveInput = 0f;
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("IsDead", true);
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowGameOver();
        }
    }

    public void ReviveAt(Vector3 revivePosition)
    {
        transform.position = revivePosition;

        isDead = false;
        isAttacking = false;
        isHurt = false;
        isSkillLOnCooldown = false;
        moveInput = 0f;

        rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.SetBool("IsDead", false);
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    public Vector3 GetRespawnPoint()
    {
        return respawnPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}