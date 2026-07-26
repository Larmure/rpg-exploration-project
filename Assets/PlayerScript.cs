using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidBody;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool isGrounded;
    private float moveInput;
    private bool jumpRequested;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        ReadMovementInput();
        ReadJumpInput();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyMovement();

        if (jumpRequested && isGrounded)
        {
            ApplyJump();
        }
        jumpRequested = false;
    }

    private void ReadMovementInput()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
        {
            jumpRequested = true;
        }
    }

    private void UpdateAnimations()
    {
        bool isWalking = moveInput != 0f;

        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }

        if (isWalking && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveInput < 0f;
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyMovement()
    {
        rigidBody.linearVelocity = new Vector2(moveInput * moveSpeed, rigidBody.linearVelocity.y);
    }

    private void ApplyJump()
    {
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}