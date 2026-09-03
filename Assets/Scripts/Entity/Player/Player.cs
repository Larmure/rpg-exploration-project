using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Entity
{

    public static Player instance;

    [Header("Stats")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float blinkInterval = 0.005f;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana = 100;
    private int invincibilityStackCount = 0;


    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private Vector2 attackOrigin = new Vector2(0f, 0.4f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Magic")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpawnOffset = 1.0f;

    [Header("Currency")]
    [SerializeField] public int gold = 0;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private float dashInvincibilityDuration = 0.15f;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float dashTimer = 0f;
    private Vector2 dashDirection;

    private Vector2 moveInput;
    private bool isAttacking = false;
    private bool isCasting = false;
    private float attackCooldownTimer = 0f;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f;
    private Vector2 attackDirection = Vector2.down;
    private Camera mainCamera;


    [Header("UI")]
    public Bar healthBar;
    public Bar manaBar;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    protected override void Start()
    {
        mainCamera = Camera.main;
        healthBar.SetMaxValue(maxHealth);
        manaBar.SetMaxValue(maxMana);
    }

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Utilisé par CliffJump.cs pour geler temporairement les contrôles pendant un saut de falaise
    private bool controlLocked = false;

    public void SetControlLocked(bool locked)
    {
        controlLocked = locked;
        if (locked) moveInput = Vector2.zero;
    }

    public Vector2 GetLastMoveDirection() => new Vector2(lastMoveX, lastMoveY);

    void Update()
    {
        if (isDead || controlLocked) return;

        HandleInput();
        UpdateAnimator();

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    private void HandleInput()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            moveInput = Vector2.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W)) vertical = 1f;
        if (Input.GetKey(KeyCode.S)) vertical = -1f;
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;

        moveInput = new Vector2(horizontal, vertical).normalized;

        bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        bool leftClick = Input.GetMouseButtonDown(0) && !pointerOverUI;

        if (leftClick)
        {
            if (!TryInteract())
            {
                if (!isAttacking && attackCooldownTimer <= 0f && !isCasting)
                {
                    Attack();
                }
            }
        }

        bool castPressed = Input.GetKeyDown(KeyCode.Space);

        if (castPressed && !isCasting && !isAttacking)
        {
            Cast();
        }

        bool usePressed = Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject();

        if (usePressed)
        {
            UseCurrentItem();
        }

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift);

        if (dashPressed && !isDashing && dashCooldownTimer <= 0f && !isAttacking && !isCasting)
        {
            StartDash();
        }
    }

    private bool TryInteract()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, interactableLayer);

        // Debug.Log($"[TryInteract] mouseWorldPos={mouseWorldPos}, hit={(hit != null ? hit.name : "null")}");

        if (hit == null) return false;

        IInteractable interactable = hit.GetComponent<IInteractable>();
        // Debug.Log($"[TryInteract] interactable={(interactable != null ? "found" : "null")}");

        if (interactable == null) return false;

        float distance = Vector2.Distance(transform.position, hit.transform.position);
        // Debug.Log($"[TryInteract] distance={distance}, range={interactionRange}");

        if (distance > interactionRange) return false;

        interactable.Interact();
        return true;
    }

    private void UseCurrentItem()
    {
        if (HotbarManager.instance == null) return;

        Item current = HotbarManager.instance.currentItem;

        if (current != null)
        {
            current.UseItem();
        }
    }

    private Vector2 GetMouseDirection()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return new Vector2(lastMoveX, lastMoveY);

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (Vector2)mouseWorldPos - (Vector2)transform.position;

        if (dir.sqrMagnitude < 0.0001f)
            return new Vector2(lastMoveX, lastMoveY);

        return dir.normalized;
    }

    private void Attack()
    {
        animator.SetBool("IsAttacking", true);
        isAttacking = true;
        moveInput = Vector2.zero;
        attackCooldownTimer = attackCooldown;

        attackDirection = GetMouseDirection();
        lastMoveX = attackDirection.x;
        lastMoveY = attackDirection.y;

        animator.SetTrigger("Attack");
        animator.SetBool("IsAttacking", true);

        Invoke(nameof(DealDamage), attackDuration * 0.5f);
    }

    private void Cast()
    {
        animator.SetBool("IsCasting", true);
        isCasting = true;
        moveInput = Vector2.zero;
        attackCooldownTimer = attackCooldown;

        attackDirection = GetMouseDirection();
        lastMoveX = attackDirection.x;
        lastMoveY = attackDirection.y;

        animator.SetTrigger("Cast");
        animator.SetBool("IsCasting", true);

        int manaCost = projectilePrefab.GetComponent<Projectile>().ManaCost;

        if (currentMana < manaCost)
        {
            EndCast();
            return;
        }

        currentMana -= manaCost;
        manaBar.SetValue(currentMana);

        Invoke(nameof(SpawnProjectile), attackDuration * 0.5f);
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        Vector2 spawnPos = (Vector2)transform.position + attackDirection * projectileSpawnOffset;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(attackDirection, enemyLayer);
    }

    private void DealDamage()
    {
        bool enemyHit = false;

        Vector2 basePos = (Vector2)transform.position + attackOrigin;
        Vector2 attackPos = basePos + attackDirection * attackDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(knockDir, attackKnockbackForce, attackKnockbackDuration);
                enemy.TakeDamage(attackDamage);
                enemyHit = true;
            }
        }

        if (enemyHit)
        {
            currentMana += 5;
            if (currentMana > maxMana) currentMana = maxMana;
            manaBar.SetValue(currentMana);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    public void EndCast()
    {
        isCasting = false;
        animator.SetBool("IsCasting", false);
    }

    private void StartDash()
    {
        Vector2 dir;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            dir = moveInput.normalized;
        }
        else
        {
            dir = -GetMouseDirection();
        }

        dashDirection = dir;
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        StartCoroutine(DashInvincibility());
    }

    private System.Collections.IEnumerator DashInvincibility()
    {
        AddInvincibility();
        yield return new WaitForSeconds(dashInvincibilityDuration);
        RemoveInvincibility();
    }

    protected override void HandleMovement()
    {
        if (controlLocked) return;

        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            return;
        }

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isAttacking || isCasting)
        {
            Vector2 dir = attackDirection;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                animator.SetFloat("MoveX", Mathf.Sign(dir.x));
                animator.SetFloat("MoveY", 0f);
            }
            else
            {
                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveY", Mathf.Sign(dir.y));
            }
        }
        else
        {
            Vector2 dir = GetMouseDirection();

            if (isMoving)
            {
                animator.SetFloat("MoveX", moveInput.x);
                animator.SetFloat("MoveY", moveInput.y);
            }

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                lastMoveX = Mathf.Sign(dir.x);
                lastMoveY = 0f;
            }
            else
            {
                lastMoveX = 0f;
                lastMoveY = Mathf.Sign(dir.y);
            }

            if (!isMoving)
            {
                animator.SetFloat("MoveX", lastMoveX * 0.1f);
                animator.SetFloat("MoveY", lastMoveY * 0.1f);
            }
        }
    }

    public override void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;

        int mitigatedDamage = Mathf.Max(amount - armorPoints, 1);

        currentHealth -= mitigatedDamage;
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBar.SetValue(currentHealth);

        // Debug.Log($"Player hit! Damage: {mitigatedDamage} (raw {amount}, armor {armorPoints}) HP remaining: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        healthBar.SetValue(currentHealth);
    }

    private void AddInvincibility()
    {
        invincibilityStackCount++;
        isInvincible = true;
    }

    private void RemoveInvincibility()
    {
        invincibilityStackCount = Mathf.Max(0, invincibilityStackCount - 1);
        isInvincible = invincibilityStackCount > 0;
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        AddInvincibility();

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        RemoveInvincibility();
    }

    public override void Die()
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

    // Debug 
    private void OnDrawGizmosSelected()
    {
        Vector2 basePos = (Vector2)transform.position + attackOrigin;
        Vector2 attackPos = basePos + attackDirection * attackDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Gold: +{amount} (total: {gold})");
        // TODO: si tu as un HUD pour l'or, appelle-le ici, ex: goldText.text = gold.ToString();
    }

    public int GetCurrentMana() => currentMana;
    public int GetMaxMana() => maxMana;
}