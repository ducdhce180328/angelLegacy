using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;
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

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;

    private bool isSliding;
    private float slideTimer;
    private float slideCooldownTimer;

    private float moveInput;
    private float verticalInput;

    private bool isAtLadderTop;
    private bool isGrounded;
    private bool isOnLadder;
    private bool isClimbing;
    private bool isFacingRight = true;

    private bool isKnockedBack;
    private float knockbackTimer;
    private float knockbackDirection;
    private float knockbackSpeed;

    private Transform currentLadderTop;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        if (slideCooldownTimer > 0f)
            slideCooldownTimer -= Time.deltaTime;

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
                isSliding = false;
        }

        HandleSlide();

        if (health != null && health.isDead) return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        CheckGround();
        HandleJump();
        HandleLadder();
        Flip();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (health != null && health.isDead) return;

        Move();
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void Move()
    {
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

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0f, verticalInput * climbSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (anim != null)
                anim.SetTrigger("Jump");
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
        }
        else
        {
            isClimbing = false;
            rb.gravityScale = normalGravity;
        }
    }

    void Flip()
    {
        if (moveInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("AirSpeedY", rb.linearVelocity.y);
        anim.SetBool("Climb", isClimbing);

        if (Mathf.Abs(moveInput) > 0.1f)
            anim.SetInteger("AnimState", 1);
        else
            anim.SetInteger("AnimState", 0);
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ladder"))
    //         isOnLadder = true;

    //     if (other.CompareTag("LadderTop"))
    //     {
    //         isAtLadderTop = true;
    //         currentLadderTop = other.transform;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ladder"))
    //     {
    //         isOnLadder = false;
    //         isClimbing = false;
    //         rb.gravityScale = normalGravity;
    //     }

    //     if (other.CompareTag("LadderTop"))
    //     {
    //         isAtLadderTop = false;
    //         currentLadderTop = null;
    //     }
    // }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
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

    void HandleSlide()
    {
        if (isSliding) return;
        if (slideCooldownTimer > 0f) return;
        if (!isGrounded) return;
        if (isClimbing) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            isSliding = true;
            slideTimer = slideDuration;
            slideCooldownTimer = slideCooldown;

            if (anim != null)
                anim.SetTrigger("Slide");
        }
    }
}