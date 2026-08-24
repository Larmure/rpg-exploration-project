using UnityEngine;

public class Firebolt : Projectile
{
    private FireboltData FireboltStats => (FireboltData)data;

    protected override void OnHitEnemy(Enemy enemy, Collider2D other)
    {
        Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
        enemy.ApplyKnockback(knockDir, FireboltStats.knockbackForce, FireboltStats.knockbackDuration);
    }
}