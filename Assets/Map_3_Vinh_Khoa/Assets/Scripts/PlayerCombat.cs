using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Basic Attack")]
    [SerializeField] private int basicAttackDamage = 10;
    [SerializeField] private float basicAttackCooldown = 0.4f;

    [Header("Basic Projectile")]
    [SerializeField] private GameObject basicProjectilePrefab;
    [SerializeField] private float basicProjectileSpeed = 8f;
    [SerializeField] private float basicProjectileMaxDistance = 5f;
    [SerializeField] private float basicProjectileSpawnOffset = 0.6f;

    [Header("Basic Spread Shot")]
    [SerializeField] private float holdToSpreadThreshold = 0.25f;
    [SerializeField] private int basicProjectileCount = 5;
    [SerializeField] private float basicSpreadAngle = 40f;

    [Header("Skill 1")]
    [SerializeField] private int skill1Damage = 25;
    [SerializeField] private float skill1Cooldown = 5f;
    [SerializeField] private int skill1ManaCost = 15;
    [SerializeField] private float skill1Radius = 1.2f;
    [SerializeField] private float skill1OffsetFromPlayer = 1.2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector3 skill1CastScale = new Vector3(1.5f, 1.5f, 1f);

    [Header("Teleport Skill")]
    [SerializeField] private float teleportDistance = 3f;
    [SerializeField] private float teleportCooldown = 8f;
    [SerializeField] private LayerMask teleportBlockLayer;
    [SerializeField] private float teleportCheckRadius = 0.2f;
    [SerializeField] private bool preventTeleportIntoWall = true;

    [Header("Teleport Fall Effect")]
    [SerializeField] private float teleportFallGravityScale = 0.8f;
    [SerializeField] private float teleportFallDuration = 0.35f;

    [Header("Teleport Combo Hold")]
    [SerializeField] private float teleportComboWindow = 0.4f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats;
    private SkillUI skillUI;
    private Rigidbody2D rb;

    private float nextAttackTime;
    private float nextSkill1Time;
    private float nextTeleportTime;

    private Vector2 cachedShootDirection = Vector2.right;

    private bool isAttacking;
    private bool hasFiredThisAttack;

    private bool isCastingSkill;
    private bool hasAppliedSkill1Damage;

    private float originalGravityScale;
    private float teleportFallEndTime;

    private bool isTeleportHoldingPlayer;
    private float teleportComboEndTime;
    private bool keepPlayerFloatingUntilSkillEnds;

    private float leftMouseHoldStartTime;
    private bool isHoldingLeftMouse;
    private bool useSpreadShotForThisAttack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        skillUI = FindFirstObjectByType<SkillUI>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
    }

    private void Update()
    {
        UpdateAimDirection();
        HandleTeleportFallEffect();
        HandleTeleportComboHold();

        if (skillUI != null)
        {
            float skill1Remain = Mathf.Max(0f, nextSkill1Time - Time.time);
            skillUI.UpdateSkill1Cooldown(skill1Remain);

            float skill2Remain = Mathf.Max(0f, nextTeleportTime - Time.time);
            skillUI.UpdateSkill2Cooldown(skill2Remain);
        }

        if (playerStats != null && playerStats.IsDead())
            return;

        if (isCastingSkill)
            return;

        if (!isAttacking && Input.GetKeyDown(KeyCode.Q))
        {
            TryCastSkill1();
            return;
        }

        if (!isAttacking && Input.GetMouseButtonDown(1))
        {
            TryTeleport();
            return;
        }

        HandleBasicAttackInput();
    }

    private void HandleBasicAttackInput()
    {
        if (isAttacking)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isHoldingLeftMouse = true;
            leftMouseHoldStartTime = Time.time;
            useSpreadShotForThisAttack = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingLeftMouse && Time.time >= nextAttackTime)
            {
                useSpreadShotForThisAttack = false;
                PrepareBasicAttack();
            }

            isHoldingLeftMouse = false;
            return;
        }

        if (isHoldingLeftMouse && Input.GetMouseButton(0))
        {
            float holdDuration = Time.time - leftMouseHoldStartTime;

            if (holdDuration >= holdToSpreadThreshold && Time.time >= nextAttackTime)
            {
                useSpreadShotForThisAttack = true;
                PrepareBasicAttack();
                isHoldingLeftMouse = false;
            }
        }
    }

    private void UpdateAimDirection()
    {
        if (Camera.main == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (Vector2)(mouseWorld - transform.position);

        if (direction.sqrMagnitude > 0.001f)
        {
            cachedShootDirection = direction.normalized;
        }
    }

    private void PrepareBasicAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        isAttacking = true;
        hasFiredThisAttack = false;
        nextAttackTime = Time.time + basicAttackCooldown;

        FaceToDirection(cachedShootDirection);

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    private void TryCastSkill1()
    {
        if (Time.time < nextSkill1Time)
            return;

        if (playerStats != null && !playerStats.UseMana(skill1ManaCost))
            return;

        isCastingSkill = true;
        hasAppliedSkill1Damage = false;
        nextSkill1Time = Time.time + skill1Cooldown;

        FaceToDirection(cachedShootDirection);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (isTeleportHoldingPlayer && Time.time <= teleportComboEndTime)
        {
            keepPlayerFloatingUntilSkillEnds = true;

            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }
        }

        animator.ResetTrigger("Skill1");
        animator.SetTrigger("Skill1");
    }

    private void TryTeleport()
    {
        if (Time.time < nextTeleportTime)
            return;

        if (Camera.main == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 rawDirection = (Vector2)(mouseWorld - transform.position);

        if (rawDirection.sqrMagnitude <= 0.001f)
            return;

        Vector2 direction = rawDirection.normalized;

        FaceToDirection(direction);

        float distanceToMouse = Vector2.Distance(currentPosition, mouseWorld);
        float finalDistance = Mathf.Min(distanceToMouse, teleportDistance);

        Vector2 targetPosition = currentPosition + direction * finalDistance;

        if (preventTeleportIntoWall)
        {
            Collider2D hit = Physics2D.OverlapCircle(targetPosition, teleportCheckRadius, teleportBlockLayer);
            if (hit != null)
                return;
        }

        if (rb != null)
        {
            rb.position = targetPosition;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            transform.position = targetPosition;
        }

        nextTeleportTime = Time.time + teleportCooldown;
        teleportFallEndTime = Time.time + teleportFallDuration;

        isTeleportHoldingPlayer = true;
        teleportComboEndTime = Time.time + teleportComboWindow;
        keepPlayerFloatingUntilSkillEnds = false;
    }

    private void HandleTeleportFallEffect()
    {
        if (rb == null)
            return;

        if (playerStats != null && playerStats.IsDead())
        {
            teleportFallEndTime = 0f;
            isTeleportHoldingPlayer = false;
            keepPlayerFloatingUntilSkillEnds = false;
            return;
        }

        if (keepPlayerFloatingUntilSkillEnds && isCastingSkill)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (teleportFallEndTime > 0f && Time.time < teleportFallEndTime)
        {
            rb.gravityScale = teleportFallGravityScale;
        }
        else
        {
            rb.gravityScale = originalGravityScale;
            teleportFallEndTime = 0f;
        }
    }

    private void HandleTeleportComboHold()
    {
        if (rb == null)
            return;

        if (keepPlayerFloatingUntilSkillEnds)
            return;

        if (!isTeleportHoldingPlayer)
            return;

        if (Time.time > teleportComboEndTime)
        {
            isTeleportHoldingPlayer = false;
            return;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void FaceToDirection(Vector2 direction)
    {
        if (direction.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    public bool IsCastingSkill()
    {
        return isCastingSkill;
    }

    public void FireBasicProjectile()
    {
        if (hasFiredThisAttack)
            return;

        hasFiredThisAttack = true;

        if (basicProjectilePrefab == null)
            return;

        if (!useSpreadShotForThisAttack)
        {
            SpawnProjectile(cachedShootDirection);
            return;
        }

        int projectileCount = Mathf.Max(1, basicProjectileCount);
        float totalSpread = basicSpreadAngle;

        float startAngle = -totalSpread * 0.5f;
        float angleStep = projectileCount > 1 ? totalSpread / (projectileCount - 1) : 0f;

        float baseAngle = Mathf.Atan2(cachedShootDirection.y, cachedShootDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = baseAngle + startAngle + angleStep * i;
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector2 shootDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
            SpawnProjectile(shootDirection);
        }
    }

    private void SpawnProjectile(Vector2 shootDirection)
    {
        Vector3 spawnPosition = transform.position + (Vector3)(shootDirection * basicProjectileSpawnOffset);

        GameObject projectileObject = Instantiate(
            basicProjectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        WizardProjectile projectile = projectileObject.GetComponent<WizardProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                shootDirection,
                basicProjectileSpeed,
                basicProjectileMaxDistance,
                basicAttackDamage,
                playerStats
            );
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        hasFiredThisAttack = false;
        useSpreadShotForThisAttack = false;
        animator.ResetTrigger("Attack");
    }

    public void ApplySkill1Damage()
    {
        if (hasAppliedSkill1Damage)
            return;

        hasAppliedSkill1Damage = true;

        Vector2 faceDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 hitCenter = (Vector2)transform.position + faceDirection * skill1OffsetFromPlayer;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitCenter, skill1Radius, enemyLayer);

        for (int i = 0; i < hitEnemies.Length; i++)
        {
            EnemyHealth enemyHealth = hitEnemies[i].GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(skill1Damage);

                if (playerStats != null)
                {
                    playerStats.HealOnHit();
                }
            }
        }
    }

    public void EndSkill1()
    {
        isCastingSkill = false;
        hasAppliedSkill1Damage = false;
        animator.ResetTrigger("Skill1");

        if (rb != null)
        {
            rb.gravityScale = originalGravityScale;
        }

        keepPlayerFloatingUntilSkillEnds = false;
        isTeleportHoldingPlayer = false;
        teleportFallEndTime = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, teleportDistance);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        bool facingLeft = sr != null && sr.flipX;

        Vector2 faceDirection = facingLeft ? Vector2.left : Vector2.right;
        Vector2 hitCenter = (Vector2)transform.position + faceDirection * skill1OffsetFromPlayer;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hitCenter, skill1Radius);
    }
}