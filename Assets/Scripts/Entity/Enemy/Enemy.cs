using UnityEngine;

public class Enemy : Entity
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Transform player;
    private float attackCooldownTimer = 0f;
    [SerializeField] private float damageDelay = 0.3f;

    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Debug.Log($"{gameObject.name} hit ! HP remaining : {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log($"{gameObject.name} is dead !");
        Destroy(gameObject);
    }

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
            Player Player = player.GetComponent<Player>();
            if (Player != null)
            {
                Vector2 knockDir = (player.position - transform.position).normalized;
                Player.ApplyKnockback(knockDir, attackKnockbackForce, attackKnockbackDuration);
                Player.TakeDamage(attackDamage);
            }
        }

    }

}