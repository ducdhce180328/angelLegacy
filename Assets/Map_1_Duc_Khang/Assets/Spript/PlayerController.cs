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

    [Header("Ladder")]
    public float climbSpeed = 4f;
    public float normalGravity = 3f;
    public float ladderExitOffsetY = 0.6f;

    [Header("Slide")]
    public float slideSpeed = 8f;
    public float slideDuration = 0.2f;
    public float slideCooldown = 1f;

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

    [Header("Gamepad Buttons")]
    public KeyCode gamepadJumpButton = KeyCode.JoystickButton0;        // A
    public KeyCode gamepadNormalAttackButton = KeyCode.JoystickButton2; // X
    public KeyCode gamepadSkillKButton = KeyCode.JoystickButton1;       // B
    public KeyCode gamepadSkillLButton = KeyCode.JoystickButton3;       // Y
    public KeyCode gamepadSlideButton = KeyCode.JoystickButton5;        // RB

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private float verticalInput;

    private bool isGrounded;
    private bool isAttacking;
    private bool isSkillLOnCooldown;

    private bool isOnLadder;
    private bool isClimbing;
    private bool isAtLadderTop;

    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;

    private bool isKnockedBack;
    private float knockbackTimer;
    private float knockbackDirection;
    private float knockbackSpeed;

    private bool isFacingRight = true;
    private Vector3 originalScale;
    private Vector3 respawnPosition;
    private Transform currentLadderTop;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        originalScale = transform.localScale;
        respawnPosition = transform.position;

        isSkillLOnCooldown = false;
        hasUnlockedSkillL = false;

        if (rb != null)
        {
            rb.gravityScale = normalGravity;
        }

        isFacingRight = transform.localScale.x >= 0;
    }
private void Start()
{
    if (PlayerSpawnData.hasSpawnPosition)
    {
        transform.position = PlayerSpawnData.spawnPosition;
        PlayerSpawnData.hasSpawnPosition = false;
    }
}
    private void Update()
    {
        CheckGround();
        UpdateTimers();

        if (isDead)
        {
            moveInput = 0f;
            verticalInput = 0f;
            UpdateAnimator();
            return;
        }

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        moveInput = GetHorizontalInput();
        verticalInput = GetVerticalInput();

        if (!isAttacking && !isHurt && !isSliding && !isKnockedBack)
        {
            if (JumpPressed() && isGrounded && !isClimbing)
            {
                Jump();
            }

            if (NormalAttackPressed())
            {
                NormalAttack();
            }

            if (SkillKPressed())
            {
                SkillAttack();
            }

            if (SkillLPressed())
            {
                UseSkillL();
            }

            if (SlidePressed())
            {
                TrySlide();
            }
        }
        else
        {
            if (isAttacking || isHurt || isSliding || isKnockedBack)
            {
                moveInput = 0f;
            }
        }

        HandleLadder();
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

        if (isKnockedBack)
        {
            rb.linearVelocity = new Vector2(knockbackDirection * knockbackSpeed, rb.linearVelocity.y);
            return;
        }

        if (isSliding)
        {
            float dir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * slideSpeed, rb.linearVelocity.y);
            return;
        }

        if (isAttacking || isHurt)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0f, verticalInput * climbSpeed);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateTimers()
    {
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false;
            }
        }
    }

    private float GetHorizontalInput()
    {
        float value = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(value) < 0.01f)
        {
            value = Input.GetAxisRaw("JoystickHorizontal");
        }

        return Mathf.Abs(value) > 0.1f ? Mathf.Sign(value) : 0f;
    }

    private float GetVerticalInput()
    {
        float value = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(value) < 0.01f)
        {
            value = Input.GetAxisRaw("JoystickVertical");
        }

        return Mathf.Abs(value) > 0.1f ? Mathf.Sign(value) : 0f;
    }

    private bool JumpPressed()
    {
        return Input.GetButtonDown("Jump")
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(gamepadJumpButton);
    }

    private bool NormalAttackPressed()
    {
        return Input.GetKeyDown(KeyCode.J)
            || Input.GetKeyDown(gamepadNormalAttackButton);
    }

    private bool SkillKPressed()
    {
        return Input.GetKeyDown(KeyCode.K)
            || Input.GetKeyDown(gamepadSkillKButton);
    }

    private bool SkillLPressed()
    {
        return Input.GetKeyDown(KeyCode.L)
            || Input.GetKeyDown(gamepadSkillLButton);
    }

    private bool SlidePressed()
    {
        return Input.GetKeyDown(KeyCode.I)
            || Input.GetKeyDown(gamepadSlideButton);
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

        // Bộ param của script cũ
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        anim.SetBool("IsDead", isDead);

        // Bộ param của script còn lại
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("AirSpeedY", rb.linearVelocity.y);
        anim.SetBool("Climb", isClimbing);

        if (Mathf.Abs(moveInput) > 0.1f)
            anim.SetInteger("AnimState", 1);
        else
            anim.SetInteger("AnimState", 0);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (anim != null)
        {
            anim.SetTrigger("Jump");
        }
    }

    void HandleLadder()
    {
        if (isOnLadder)
        {
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                isClimbing = true;
                rb.gravityScale = 0f;
            }

            if (isClimbing)
            {
                rb.gravityScale = 0f;
            }

            if (isAtLadderTop && verticalInput > 0.1f && currentLadderTop != null)
            {
                ExitLadderAtTop();
            }

            if (!isOnLadder)
            {
                isClimbing = false;
                rb.gravityScale = normalGravity;
            }
        }
        else
        {
            isClimbing = false;
            rb.gravityScale = normalGravity;
        }
    }

    void ExitLadderAtTop()
    {
        isClimbing = false;
        isOnLadder = false;

        rb.gravityScale = normalGravity;
        rb.linearVelocity = Vector2.zero;

        Vector3 pos = transform.position;
        pos.y = currentLadderTop.position.y + ladderExitOffsetY;
        transform.position = pos;
    }

    void Flip()
    {
        if (moveInput > 0)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void TrySlide()
    {
        if (isSliding) return;
        if (slideCooldownTimer > 0f) return;
        if (!isGrounded) return;
        if (isClimbing) return;
        if (isAttacking || isHurt || isDead) return;

        isSliding = true;
        slideTimer = slideDuration;
        slideCooldownTimer = slideCooldown;

        if (anim != null)
        {
            anim.SetTrigger("Slide");
        }
    }

    void NormalAttack()
    {
        if (isDead || isAttacking || isHurt || isSliding || isClimbing) return;
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
        if (isDead || isAttacking || isHurt || isSkillLOnCooldown || isSliding || isClimbing) return;

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
        if (isDead || isAttacking || isHurt || isSliding || isClimbing) return;

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
        float direction = isFacingRight ? 1f : -1f;

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
        float direction = isFacingRight ? 1f : -1f;

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
        isSliding = false;
        isClimbing = false;
        isKnockedBack = false;
        moveInput = 0f;
        verticalInput = 0f;

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
        isSliding = false;
        isClimbing = false;
        isKnockedBack = false;
        isSkillLOnCooldown = false;
        moveInput = 0f;
        verticalInput = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = normalGravity;

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

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public void ApplyKnockback(float direction, float speed, float duration)
    {
        isKnockedBack = true;
        knockbackDirection = direction;
        knockbackSpeed = speed;
        knockbackTimer = duration;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }

        if (other.CompareTag("LadderTop"))
        {
            isAtLadderTop = true;
            currentLadderTop = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;
            rb.gravityScale = normalGravity;
        }

        if (other.CompareTag("LadderTop"))
        {
            isAtLadderTop = false;
            currentLadderTop = null;
        }
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