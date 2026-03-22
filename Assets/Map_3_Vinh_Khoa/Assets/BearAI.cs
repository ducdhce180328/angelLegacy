using UnityEngine;

public class BearAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Move")]
    public float moveSpeed = 2f;

    [Header("State")]
    public bool playerInDetectRange = false;
    public bool playerInAttackRange = false;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    private float attackTimer = 0f;

    [Header("Refs")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public BearHealth bearHealth;

    private bool isDead = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (bearHealth == null) bearHealth = GetComponent<BearHealth>();
    }

    private void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        // Không có player hoặc player ra khỏi detect range => Idle
        if (target == null || !playerInDetectRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        // Player ở trong vùng attack => Attack
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
            // Player còn trong detect nhưng chưa đủ gần => Move
            animator.SetBool("isAttacking", false);
            animator.SetBool("isMoving", true);

            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }

    private void FaceTarget()
    {
        if (target == null) return;

        if (target.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (target.position.x < transform.position.x)
            spriteRenderer.flipX = true;
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