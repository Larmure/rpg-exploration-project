using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public static PlayerScript instance;

    [Header("Stats")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth = 100;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool isInvincible = false;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f;
    private bool isDead = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

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

        Invoke(nameof(DealDamage), attackDuration * 0.5f);
        Invoke(nameof(EndAttack), attackDuration);
    }

    private void DealDamage()
    {
        Vector2 attackDir = new Vector2(lastMoveX, lastMoveY);
        Vector2 attackPos = (Vector2)transform.position + attackDir * attackDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyScript enemy = hit.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isAttacking)
        {
            animator.SetFloat("MoveX", lastMoveX);
            animator.SetFloat("MoveY", lastMoveY);
        }
        else if (isMoving)
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
        if (isInvincible || isDead) return;

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
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        if (animator != null)
            animator.SetTrigger("Death");

        // Plus de délai en dur — c'est l'Animation Event qui appellera
        // OnDeathAnimationComplete() à la fin du clip, quelle que soit sa durée
    }

    // Appelée automatiquement par l'Animation Event placé sur la dernière frame du clip "Death"
    public void OnDeathAnimationComplete()
    {
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.TriggerGameOver();
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    // Debug 
    private void OnDrawGizmosSelected()
    {
        Vector2 attackDir = new Vector2(lastMoveX, lastMoveY);
        Vector2 attackPos = (Vector2)transform.position + attackDir * attackDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }
}