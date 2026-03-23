using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private const float PlayerRefreshInterval = 0.5f;
    private const float PatrolPointOverlapThreshold = 0.05f;
    private const float FallbackPatrolDistance = 1.5f;

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
    private Vector2 fallbackPointAPosition;
    private Vector2 fallbackPointBPosition;
    private bool useFallbackPatrolPoints;
    private bool moveTowardsFallbackPointB;
    private float playerRefreshTimer;

    private float attackTimer;
    private bool isDead;
    private bool isAttacking;
    private bool isKnockedBack;
    private float knockbackTimer;
    private float knockbackDirection;
    private float knockbackSpeed;

    private void Start()
    {
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        InitializePatrolRoute();
        RefreshPlayerReference(true);

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

        if (isKnockedBack)
        {
            HandleKnockback();
            UpdateAnimation(0f);
            return;
        }

        RefreshPlayerReference();
        attackTimer += Time.deltaTime;

        if (isAttacking)
        {
            StopMoving();
            UpdateAnimation(0f);
            return;
        }

        if (!HasPatrolRoute())
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
        Vector2 patrolTarget = GetCurrentPatrolTarget();
        MoveTo(patrolTarget, patrolSpeed);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.05f)
        {
            SwitchPatrolTarget();
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

    void HandleKnockback()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(knockbackDirection * knockbackSpeed, rb.linearVelocity.y);
        }

        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            StopMoving();
        }
    }

    void UpdateAnimation(float speedValue)
    {
        if (anim == null) return;

        anim.SetFloat("Speed", speedValue);
    }

    public void DealDamageToPlayer()
    {
        if (player == null || enemyHealth == null || isDead || isKnockedBack || !isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange + 0.3f) return;

        if (PlayerCompatibilityUtility.TryTakeDamage(player, enemyHealth.damageToPlayer))
        {
            float dir = player.position.x < transform.position.x ? -1f : 1f;
            PlayerCompatibilityUtility.ApplyKnockback(player, dir, 3f, 0.15f);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void ApplyKnockback(float direction, float speed, float duration)
    {
        if (isDead) return;

        isKnockedBack = true;
        isAttacking = false;
        attackTimer = 0f;
        knockbackDirection = direction;
        knockbackSpeed = speed;
        knockbackTimer = duration;

        if (anim != null)
        {
            anim.ResetTrigger("Attack");
        }
    }

    private void RefreshPlayerReference(bool force = false)
    {
        if (!force && player != null && player.gameObject.activeInHierarchy && !PlayerCompatibilityUtility.IsDead(player.gameObject))
        {
            return;
        }

        if (!force)
        {
            playerRefreshTimer -= Time.deltaTime;
            if (playerRefreshTimer > 0f)
            {
                return;
            }
        }

        playerRefreshTimer = PlayerRefreshInterval;

        GameObject playerObj = PlayerCompatibilityUtility.FindPlayer();
        player = playerObj != null ? playerObj.transform : null;
    }

    private void InitializePatrolRoute()
    {
        useFallbackPatrolPoints = pointA == null || pointB == null;

        if (!useFallbackPatrolPoints)
        {
            useFallbackPatrolPoints = Vector2.Distance(pointA.position, pointB.position) <= PatrolPointOverlapThreshold;
        }

        if (useFallbackPatrolPoints)
        {
            fallbackPointAPosition = (Vector2)transform.position + Vector2.left * FallbackPatrolDistance;
            fallbackPointBPosition = (Vector2)transform.position + Vector2.right * FallbackPatrolDistance;
            moveTowardsFallbackPointB = true;
            targetPoint = null;
            return;
        }

        targetPoint = pointB != null ? pointB : pointA;
    }

    private bool HasPatrolRoute()
    {
        return useFallbackPatrolPoints || (pointA != null && pointB != null);
    }

    private Vector2 GetCurrentPatrolTarget()
    {
        if (useFallbackPatrolPoints)
        {
            return moveTowardsFallbackPointB ? fallbackPointBPosition : fallbackPointAPosition;
        }

        return targetPoint != null ? targetPoint.position : (Vector2)transform.position;
    }

    private void SwitchPatrolTarget()
    {
        if (useFallbackPatrolPoints)
        {
            moveTowardsFallbackPointB = !moveTowardsFallbackPointB;
            return;
        }

        if (targetPoint == null)
        {
            targetPoint = pointB != null ? pointB : pointA;
            return;
        }

        targetPoint = targetPoint == pointA ? pointB : pointA;
    }
}
