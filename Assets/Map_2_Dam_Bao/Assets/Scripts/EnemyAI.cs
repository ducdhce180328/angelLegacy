using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Target")]
    public Transform player;
    public Transform attackPoint;

    [Header("Move Speed")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    [Header("Ranges")]
    public float detectRange = 2.5f;
    public float attackTriggerRange = 1.2f;
    public float damageRange = 0.9f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 1.2f;
    public LayerMask playerLayer;

    [Header("Push Back")]
    public float pushBackSpeed = 4f;
    public float pushBackDuration = 0.15f;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;

    private Transform currentPatrolTarget;
    private float attackTimer;
    private bool isFacingRight = true;
    private bool isKnockedBack;
    private float knockbackTimer;
    private float knockbackDirection;
    private float knockbackSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        currentPatrolTarget = rightPoint;

        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().flipX = false;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (health != null && health.isDead) return;
        if (leftPoint == null || rightPoint == null) return;

        if (isKnockedBack)
        {
            HandleKnockback();
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackTriggerRange)
        {
            HandleAttack();
        }
        else if (distanceToPlayer <= detectRange)
        {
            HandleChase();
        }
        else
        {
            HandlePatrol();
        }
    }

    void HandlePatrol()
    {
        float dir = Mathf.Sign(currentPatrolTarget.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);

        if (anim != null)
            anim.SetFloat("Speed", 1f);

        if (Mathf.Abs(transform.position.x - currentPatrolTarget.position.x) <= 0.1f)
        {
            if (currentPatrolTarget == rightPoint)
            {
                currentPatrolTarget = leftPoint;
                FaceLeft();
            }
            else
            {
                currentPatrolTarget = rightPoint;
                FaceRight();
            }
        }
        else
        {
            FaceTarget(currentPatrolTarget.position.x);
        }
    }

    void HandleChase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * runSpeed, rb.linearVelocity.y);

        if (anim != null)
            anim.SetFloat("Speed", 3f);

        FaceTarget(player.position.x);
    }

    void HandleAttack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
            anim.SetFloat("Speed", 0f);

        FaceTarget(player.position.x);

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;

            if (anim != null)
                anim.SetTrigger("Attack");

            DealDamage();
        }
    }

    public void DealDamage()
    {
        if (attackPoint == null || isKnockedBack) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, damageRange, playerLayer);
        if (hit == null) return;

        bool damaged = PlayerCompatibilityUtility.TryTakeDamage(hit, damage);
        if (damaged)
        {
            float dir = hit.transform.position.x < transform.position.x ? -1f : 1f;
            PlayerCompatibilityUtility.ApplyKnockback(hit, dir, pushBackSpeed, pushBackDuration);
        }
    }

    void HandleKnockback()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(knockbackDirection * knockbackSpeed, rb.linearVelocity.y);
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }

        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    void FaceTarget(float targetX)
    {
        if (targetX > transform.position.x)
            FaceRight();
        else if (targetX < transform.position.x)
            FaceLeft();
    }

    void FaceRight()
    {
        if (!isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void FaceLeft()
    {
        if (isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void ApplyKnockback(float direction, float speed, float duration)
    {
        if (health != null && health.isDead) return;

        isKnockedBack = true;
        attackTimer = attackCooldown;
        knockbackDirection = direction;
        knockbackSpeed = speed;
        knockbackTimer = duration;

        if (anim != null)
        {
            anim.ResetTrigger("Attack");
            anim.SetFloat("Speed", 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackTriggerRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, damageRange);
        }

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }
    }
}
