using UnityEngine;

public class Enemy : Entity
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    private Transform player;
    private float attackCooldownTimer = 0f;
    [SerializeField] private float damageDelay = 0.3f;

    [Header("Chase")]
    [SerializeField] private float directionUpdateInterval = 1f;
    [SerializeField] private float pauseBeforeDirectionChange = 0.5f;
    [SerializeField] private float sleepDelay = 3f;
    private Vector2 currentMoveDirection = Vector2.zero;
    private float directionUpdateTimer = 0f;
    private bool isPausingBeforeChange = false;
    private bool playerDetected = false;
    private float pauseTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        FindPlayer();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning($"{gameObject.name}: no player found in the scene.");
    }

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
        CancelInvoke(nameof(GoToSleep));
        if (!playerDetected)
        {
            animator.ResetTrigger("GoToSleep");
            animator.SetTrigger("PlayerNear");
            currentMoveDirection = (player.position - transform.position).normalized;
            directionUpdateTimer = directionUpdateInterval;
        }

        playerDetected = true;
    }

    public void PlayerLost()
    {
        Invoke(nameof(GoToSleep), sleepDelay);
    }

    private void GoToSleep()
    {
        playerDetected = false;
        isPausingBeforeChange = false;
        currentMoveDirection = Vector2.zero;
        animator.ResetTrigger("PlayerNear");
        animator.SetTrigger("GoToSleep");
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

    protected override void HandleMovement()
    {
        if (GameOverManager.IsGameOver) return;
        if (!playerDetected || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.2f) return;

        if (isPausingBeforeChange)
        {
            pauseTimer -= Time.fixedDeltaTime;

            if (pauseTimer <= 0f)
            {
                isPausingBeforeChange = false;
                currentMoveDirection = (player.position - transform.position).normalized;
                directionUpdateTimer = directionUpdateInterval;

                rb.MovePosition(rb.position + currentMoveDirection * GetCurrentMoveSpeed() * Time.fixedDeltaTime);
                animator.SetFloat("MoveX", currentMoveDirection.x);
                animator.SetFloat("MoveY", currentMoveDirection.y);
            }

            return;
        }

        directionUpdateTimer -= Time.fixedDeltaTime;
        if (directionUpdateTimer <= 0f)
        {
            isPausingBeforeChange = true;
            pauseTimer = pauseBeforeDirectionChange;
            return;
        }

        rb.MovePosition(rb.position + currentMoveDirection * GetCurrentMoveSpeed() * Time.fixedDeltaTime);

        animator.SetFloat("MoveX", currentMoveDirection.x);
        animator.SetFloat("MoveY", currentMoveDirection.y);
    }

}