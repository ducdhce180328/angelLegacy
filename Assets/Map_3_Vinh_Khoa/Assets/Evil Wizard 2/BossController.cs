using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform skillAttackPoint;
    [SerializeField] private LayerMask groundLayer;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float stopChaseRange = 12f;
    [SerializeField] private float maxChaseDistanceFromHome = 10f;

    [Header("Attack 1 - Melee")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackRadius = 0.8f;

    [Header("Attack 2 - Teleport Skill")]
    [SerializeField] private float skillMinRange = 3f;
    [SerializeField] private float skillMaxRange = 8f;
    [SerializeField] private int attack2Damage = 25;
    [SerializeField] private float attack2Cooldown = 4f;
    [SerializeField] private float attack2Radius = 1f;
    [SerializeField] private float teleportOffsetFromPlayer = 1.2f;

    [Header("Target Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Teleport Check")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float teleportGroundCheckDistance = 2f;
    [SerializeField] private Vector2 teleportBodyCheckSize = new Vector2(1.2f, 2.5f);
    [SerializeField] private float teleportYOffset = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;
    private Collider2D bodyCollider;

    private Vector2 homePosition;
    private bool isGrounded;
    private bool isDead;
    private bool isAttacking;

    private float attackCooldownTimer;
    private float attack2CooldownTimer;

    public bool IsDead => isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        bodyCollider = GetComponent<Collider2D>();

        homePosition = transform.position;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (player == null) return;

        attackCooldownTimer -= Time.deltaTime;
        attack2CooldownTimer -= Time.deltaTime;

        CheckGround();
        HandleFacing();
        HandleCombat();
        UpdateAnimator();
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleCombat()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceFromHome = Vector2.Distance(transform.position, homePosition);

        bool insideChaseZone = distanceFromHome <= maxChaseDistanceFromHome;
        bool detectedPlayer = distanceToPlayer <= detectionRange;
        bool inAttack1Range = distanceToPlayer <= attackRange;
        bool canUseSkill = distanceToPlayer >= skillMinRange && distanceToPlayer <= skillMaxRange;

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (detectedPlayer && insideChaseZone)
        {
            if (inAttack1Range)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                if (attackCooldownTimer <= 0f)
                {
                    StartAttack1();
                }
            }
            else if (canUseSkill && attack2CooldownTimer <= 0f)
            {
                StartTeleportAttack2();
            }
            else
            {
                MoveToTarget(player.position.x);
            }
        }
        else
        {
            if (distanceToPlayer > stopChaseRange || !insideChaseZone)
            {
                MoveToTarget(homePosition.x);
            }
            else
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    private void MoveToTarget(float targetX)
    {
        float direction = Mathf.Sign(targetX - transform.position.x);
        float xDistance = Mathf.Abs(targetX - transform.position.x);

        if (xDistance > 0.1f)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void HandleFacing()
    {
        if (player == null) return;

        if (isAttacking)
        {
            if (player.position.x > transform.position.x)
                spriteRenderer.flipX = false;
            else if (player.position.x < transform.position.x)
                spriteRenderer.flipX = true;

            return;
        }

        if (rb.linearVelocity.x > 0.05f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.05f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void StartAttack1()
    {
        if (isAttacking) return;

        isAttacking = true;
        attackCooldownTimer = attackCooldown;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack1");
        }
    }

    private void StartTeleportAttack2()
    {
        if (isAttacking) return;

        bool teleported = TryTeleportNearPlayer();

        if (!teleported)
        {
            return;
        }

        isAttacking = true;
        attack2CooldownTimer = attack2Cooldown;

        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack2");
        }
    }

    private bool TryTeleportNearPlayer()
    {
        if (player == null) return false;

        Vector3 leftCandidate = new Vector3(
            player.position.x - teleportOffsetFromPlayer,
            player.position.y + 1f,
            transform.position.z
        );

        Vector3 rightCandidate = new Vector3(
            player.position.x + teleportOffsetFromPlayer,
            player.position.y + 1f,
            transform.position.z
        );

        bool preferLeft = transform.position.x < player.position.x;

        Vector3 firstTry = preferLeft ? leftCandidate : rightCandidate;
        Vector3 secondTry = preferLeft ? rightCandidate : leftCandidate;

        if (TryPlaceBossAtCandidate(firstTry))
            return true;

        if (TryPlaceBossAtCandidate(secondTry))
            return true;

        return false;
    }

    private bool TryPlaceBossAtCandidate(Vector3 candidateTop)
    {
        if (bodyCollider == null) return false;

        RaycastHit2D groundHit = Physics2D.Raycast(
            candidateTop,
            Vector2.down,
            teleportGroundCheckDistance,
            groundLayer
        );

        if (groundHit.collider == null)
            return false;

        float colliderHalfHeight = bodyCollider.bounds.extents.y;
        float colliderCenterOffsetY = bodyCollider.bounds.center.y - transform.position.y;

        float finalX = candidateTop.x;
        float finalY = groundHit.point.y + colliderHalfHeight - colliderCenterOffsetY + teleportYOffset;

        Vector2 overlapCenter = new Vector2(finalX, finalY + colliderCenterOffsetY);

        Collider2D blocked = Physics2D.OverlapBox(
            overlapCenter,
            bodyCollider.bounds.size * 0.9f,
            0f,
            obstacleLayer
        );

        if (blocked != null)
            return false;

        rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(finalX, finalY, transform.position.z);

        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)homePosition : transform.position, maxChaseDistanceFromHome);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Vector3 localOffset = attackPoint.localPosition;

            if (spriteRenderer != null && spriteRenderer.flipX)
                localOffset.x = -Mathf.Abs(localOffset.x);
            else
                localOffset.x = Mathf.Abs(localOffset.x);

            Vector3 attackCenter = transform.TransformPoint(localOffset);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackCenter, attackRadius);
        }

        if (skillAttackPoint != null)
        {
            Vector3 localOffset = skillAttackPoint.localPosition;

            if (spriteRenderer != null && spriteRenderer.flipX)
                localOffset.x = -Mathf.Abs(localOffset.x);
            else
                localOffset.x = Mathf.Abs(localOffset.x);

            Vector3 attackCenter = transform.TransformPoint(localOffset);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackCenter, attack2Radius);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, teleportBodyCheckSize);
    }

    public void DealAttackDamage()
    {
        if (player == null || attackPoint == null) return;

        Vector3 localOffset = attackPoint.localPosition;

        if (spriteRenderer.flipX)
            localOffset.x = -Mathf.Abs(localOffset.x);
        else
            localOffset.x = Mathf.Abs(localOffset.x);

        Vector3 attackCenter = transform.TransformPoint(localOffset);

        Collider2D hit = Physics2D.OverlapCircle(
            attackCenter,
            attackRadius,
            playerLayer
        );

        if (hit != null)
        {
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(attackDamage);
                Debug.Log("Boss hit player with Attack1: " + attackDamage);
            }
        }
    }

    public void DealAttack2Damage()
    {
        if (player == null || skillAttackPoint == null) return;

        Vector3 localOffset = skillAttackPoint.localPosition;

        if (spriteRenderer.flipX)
            localOffset.x = -Mathf.Abs(localOffset.x);
        else
            localOffset.x = Mathf.Abs(localOffset.x);

        Vector3 attackCenter = transform.TransformPoint(localOffset);

        Collider2D hit = Physics2D.OverlapCircle(
            attackCenter,
            attack2Radius,
            playerLayer
        );

        if (hit != null)
        {
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(attack2Damage);
                Debug.Log("Boss hit player with Attack2: " + attack2Damage);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack1");
        }
    }

    public void EndAttack2()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack2");
        }
    }

    public void OnTakeHit()
    {
        isAttacking = false;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
        }
    }

    public void EndTakeHit()
    {
        if (animator != null)
        {
            animator.ResetTrigger("TakeHit");
        }

        isAttacking = false;
    }

    private void UpdateAnimator()
    {
        if (animator == null || rb == null) return;

        float moveSpeedValue = Mathf.Abs(rb.linearVelocity.x);

        if (moveSpeedValue < 0.05f)
            moveSpeedValue = 0f;

        animator.SetFloat("Speed", moveSpeedValue);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);
    }

    public void OnDeath()
    {
        isDead = true;
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;
    }

}