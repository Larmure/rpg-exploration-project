using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int armorPoints = 0;
    [SerializeField] protected float moveSpeed = 5f;

    [SerializeField] protected float attackKnockbackForce = 8f;
    [SerializeField] protected float attackKnockbackDuration = 0.2f; 


    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool isDead = false;
    protected SpriteRenderer spriteRenderer;

    [Header("Attack")]
    [SerializeField] protected int attackDamage = 10;

    [Header("Knockback")]
    protected bool isKnockedBack = false;
    protected Vector2 knockbackStartVelocity;
    protected float knockbackTimer = 0f;
    protected float knockbackDuration = 0f;

    [Header("Slow")]
    [SerializeField] protected Color slowTintColor = new Color(0.5f, 0.8f, 1f, 1f);
    protected bool isSlowed = false;
    protected float slowMultiplier = 1f;
    protected float slowTimer = 0f;
    protected float slowDuration = 0f;
    private Color baseColor;

    protected bool isInvincible = false;

    protected virtual void Awake()
    {
        // if(instance == null)
        // {
        //     instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} needs a SpriteRenderer.");
        }
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        if (isSlowed)
        {
            HandleSlow();
        }

        if (isKnockedBack)
        {
            HandleKnockback();
        }
        else
        {
            HandleMovement();
        }
    }

    protected virtual void HandleMovement() { }

    private void HandleKnockback()
    {
        knockbackTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(knockbackTimer / knockbackDuration);
        float easedMultiplier = Mathf.Pow(1f - t, 2f);

        Vector2 currentKnockbackVelocity = knockbackStartVelocity * easedMultiplier;
        rb.MovePosition(rb.position + currentKnockbackVelocity * Time.fixedDeltaTime);

        if (t >= 1f)
        {
            isKnockedBack = false;
        }
    }

    private void HandleSlow()
    {
        slowTimer += Time.fixedDeltaTime;

        if (slowTimer >= slowDuration)
        {
            isSlowed = false;
            slowMultiplier = 1f;
            SetTint(baseColor);
        }
    }

    public virtual void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (isDead || isInvincible) return;

        isKnockedBack = true;
        knockbackStartVelocity = direction.normalized * force;
        knockbackDuration = duration;
        knockbackTimer = 0f;
    }

    public virtual void ApplySlow(float multiplier, float duration)
    {
        if (isDead || isInvincible) return;

        if (isSlowed)
        {
            multiplier = Mathf.Min(multiplier, slowMultiplier);
        }

        isSlowed = true;
        slowMultiplier = Mathf.Clamp01(multiplier);
        slowDuration = duration;
        slowTimer = 0f;
        SetTint(slowTintColor);
        //Debug.Log($"Entity {gameObject.name} is slowed: multiplier={slowMultiplier}, duration={slowDuration}");
    }

    protected void SetTint(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public abstract void TakeDamage(int amount);

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public abstract void Die();

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetArmorPoints() => armorPoints;
    public void SetArmorPoints(int value) => armorPoints = value;
    protected float GetCurrentMoveSpeed()
    {
        return moveSpeed * (isSlowed ? slowMultiplier : 1f);
    }
}