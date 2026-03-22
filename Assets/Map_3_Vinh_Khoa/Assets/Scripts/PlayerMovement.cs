using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerStats playerStats;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerCombat playerCombat;

    private float moveInput;
    private bool isRunning;
    private bool isGrounded;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (playerStats != null && playerStats.IsDead())
        {
            moveInput = 0f;
            isRunning = false;
            jumpPressed = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            UpdateAnimator();
            return;
        }

        if (playerCombat != null && playerCombat.IsCastingSkill())
        {
            moveInput = 0f;
            isRunning = false;
            jumpPressed = false;

            UpdateAnimator();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    && Mathf.Abs(moveInput) > 0.1f;

        if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (playerStats != null && playerStats.IsDead())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (playerCombat != null && playerCombat.IsCastingSkill())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null || rb == null)
            return;

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}