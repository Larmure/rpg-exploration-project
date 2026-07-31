using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.13f; // A changer en fonction de la durée de l'animation etc...s
    [SerializeField] private float attackCooldown = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool isInvincible = false;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleInput();
        UpdateAnimator();

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

    private void HandleInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (!isAttacking)
        {
            if (Input.GetKey(KeyCode.W)) vertical = 1f;
            if (Input.GetKey(KeyCode.S)) vertical = -1f;
            if (Input.GetKey(KeyCode.A)) horizontal = -1f;
            if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        }

        moveInput = new Vector2(horizontal, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && attackCooldownTimer <= 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        isAttacking = true;
        moveInput = Vector2.zero; 
        attackCooldownTimer = attackCooldown;

        animator.SetTrigger("Attack");

        Invoke(nameof(EndAttack), attackDuration);
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);

            lastMoveX = moveInput.x;
            lastMoveY = moveInput.y;
        }
        else
        {
            animator.SetFloat("MoveX", lastMoveX * 0.1f);
            animator.SetFloat("MoveY", lastMoveY * 0.1f);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player hit! HP remaining: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}