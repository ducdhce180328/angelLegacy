using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public Transform pointA;
    public Transform pointB;

    [Header("Chase")]
    public bool canChasePlayer = true;
    public float chaseSpeed = 3f;
    public float detectRange = 4f;

    [Header("Attack")]
    public bool canAttackPlayer = true;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.2f;

    private Transform targetPoint;
    private Transform player;
    private Vector3 originalScale;
    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;

    private float attackTimer;
    private bool isDead;
    private bool isAttacking;

    private void Start()
    {
        targetPoint = pointB;
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    private void Update()
    {
        if (enemyHealth != null && enemyHealth.currentHealth <= 0)
        {
            isDead = true;
        }

        if (isDead)
        {
            StopMoving();
            UpdateAnimation(0f);
            return;
        }

        attackTimer += Time.deltaTime;

        if (isAttacking)
        {
            StopMoving();
            UpdateAnimation(0f);
            return;
        }

        if (pointA == null || pointB == null)
        {
            StopMoving();
            UpdateAnimation(0f);
            return;
        }

       if (player != null)
{
    float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

    if (canAttackPlayer && distanceToPlayer <= attackRange && verticalDistance <= 1f)
    {
        AttackPlayer();
        return;
    }

    if (canChasePlayer && distanceToPlayer <= detectRange && verticalDistance <= 1f)
    {
        ChasePlayer();
        return;
    }
}
        Patrol();
    }

    void Patrol()
    {
        MoveTo(targetPoint.position, patrolSpeed);

        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance < 0.05f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        MoveTo(player.position, chaseSpeed);
    }

    void AttackPlayer()
    {
        StopMoving();
        UpdateAnimation(0f);

        if (player != null)
        {
            FaceTarget(player.position);
        }

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            isAttacking = true;

            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    void MoveTo(Vector2 targetPosition, float speed)
    {
        Vector2 delta = targetPosition - (Vector2)transform.position;

        if (delta.sqrMagnitude < 0.0001f)
        {
            StopMoving();
            UpdateAnimation(0f);
            return;
        }

        Vector2 direction = delta.normalized;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }

        if (direction.x > 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (direction.x < -0.01f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }

        UpdateAnimation(1f);
    }

    void FaceTarget(Vector2 targetPosition)
    {
        if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void UpdateAnimation(float speedValue)
    {
        if (anim == null) return;

        anim.SetFloat("Speed", speedValue);
    }

    // Gọi bằng Animation Event trong clip Attack của quái
    public void DealDamageToPlayer()
    {
        if (player == null || enemyHealth == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange + 0.3f) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(enemyHealth.damageToPlayer);
        }
    }
    // Gọi bằng Animation Event ở frame cuối clip Attack
    public void EndAttack()
    {
        isAttacking = false;
    }
}