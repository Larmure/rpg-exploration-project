using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    [Header("Slow")]
    [SerializeField] private Color slowTintColor = new Color(0.5f, 0.8f, 1f, 1f);
    private bool isSlowed = false;
    private float slowMultiplier = 1f;
    private float slowTimer = 0f;
    private float slowDuration = 0f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private bool isInvincible = false; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }
    }

    public void Tick(float deltaTime)
    {
        if (!isSlowed) return;

        slowTimer += deltaTime;
        if (slowTimer >= slowDuration)
        {
            isSlowed = false;
            slowMultiplier = 1f;
            SetTint(baseColor);
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (isInvincible) return;

        if (isSlowed)
        {
            multiplier = Mathf.Min(multiplier, slowMultiplier);
        }

        isSlowed = true;
        slowMultiplier = Mathf.Clamp01(multiplier);
        slowDuration = duration;
        slowTimer = 0f;
        SetTint(slowTintColor);
    }

    public void SetInvincible(bool value) => isInvincible = value;
    public float GetSpeedMultiplier() => isSlowed ? slowMultiplier : 1f;

    private void SetTint(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }
}