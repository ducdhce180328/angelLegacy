using UnityEngine;

public class WolfAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Move")]
    public float moveSpeed = 2.5f;

    [Header("State")]
    public bool playerInDetectRange = false;
    public bool playerInAttackRange = false;

    [Header("Attack")]
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    [Header("Refs")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public WolfHealth wolfHealth;

    private bool isDead = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (wolfHealth == null) wolfHealth = GetComponent<WolfHealth>();
    }

    private void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        // Chưa phát hiện player => Idle
        if (target == null || !playerInDetectRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        // Đã vào tầm attack => Attack
        if (playerInAttackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isMoving", false);

            if (attackTimer <= 0f)
            {
                animator.SetBool("isAttacking", true);
                attackTimer = attackCooldown;
            }
            else
            {
                animator.SetBool("isAttacking", false);
            }

            FaceTarget();
        }
        else
        {
            // Còn trong detect nhưng chưa đủ gần => Move
            animator.SetBool("isAttacking", false);
            animator.SetBool("isMoving", true);

            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            if (direction.x > 0)
                spriteRenderer.flipX = true;
            else if (direction.x < 0)
                spriteRenderer.flipX = false;
        }
    }

    private void FaceTarget()
    {
        if (target == null) return;

        if (target.position.x > transform.position.x)
            spriteRenderer.flipX = true;
        else if (target.position.x < transform.position.x)
            spriteRenderer.flipX = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", true);
    }
}