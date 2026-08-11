using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Stats")]
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Transform player;
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;
    private Animator animator;
    private float attackCooldownTimer = 0f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float damageDelay = 0.3f;

    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.2f; 
    private Rigidbody2D rb;

    [Header("Received Knockback")]
    private bool isKnockedBack = false;
    private Vector2 knockbackStartVelocity;
    private float knockbackTimer = 0f;
    private float receivedKnockbackDuration = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} hit ! HP remaining : {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} is dead !");
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    public void PlayerDetected()
    {
        animator.SetTrigger("PlayerNear");
    }

    void Update()
    {
        if (GameOverManager.IsGameOver) return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.2f)
        {
            AttackPlayer();
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            knockbackTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(knockbackTimer / receivedKnockbackDuration);
            float easedMultiplier = Mathf.Pow(1f - t, 2f);

            Vector2 currentKnockbackVelocity = knockbackStartVelocity * easedMultiplier;
            rb.MovePosition(rb.position + currentKnockbackVelocity * Time.fixedDeltaTime);

            if (t >= 1f)
            {
                isKnockedBack = false;
            }
        }
    }

    private void AttackPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // Debug.Log($"Attack direction: X={direction.x:F2}, Y={direction.y:F2}");

        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetTrigger("Attack");

        attackCooldownTimer = attackCooldown;

        Invoke(nameof(DealDamage), damageDelay);
    }

    private void DealDamage()
    {
        if (GameOverManager.IsGameOver) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        // Debug.Log($"DealDamage appelé - distance: {distance}, attackRange: {attackRange}");
        
        // float dx = Mathf.Abs(transform.position.x - player.position.x);
        // float dy = Mathf.Abs(transform.position.y - player.position.y);
        // Debug.Log($"dx={dx:F3} dy={dy:F3} distance={Vector2.Distance(transform.position, player.position):F3}");
        
        if (distance <= attackRange + 0.2f)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                Vector2 knockDir = (player.position - transform.position).normalized;
                playerScript.ApplyKnockback(knockDir, knockbackForce, knockbackDuration);
                playerScript.TakeDamage(attackDamage);
            }
        }
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        knockbackStartVelocity = direction.normalized * force;
        receivedKnockbackDuration = duration;
        knockbackTimer = 0f;
    }   
}