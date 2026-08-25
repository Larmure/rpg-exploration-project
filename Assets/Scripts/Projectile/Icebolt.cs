using UnityEngine;

public class Icebolt : Projectile
{
    private IceboltData IceboltStats => (IceboltData)data;

    protected override void OnHitEnemy(Enemy enemy, Collider2D other)
    {
        Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
        enemy.ApplyKnockback(knockDir, IceboltStats.knockbackForce, IceboltStats.knockbackDuration);

        var statusEffects = enemy.GetComponent<StatusEffectController>();
        if (statusEffects != null)
        {
            statusEffects.ApplySlow(IceboltStats.slowMultiplier, IceboltStats.slowDuration);
        }
    }
}