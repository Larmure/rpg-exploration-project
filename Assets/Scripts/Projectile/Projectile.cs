using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected SpellData data;

    public int ManaCost => data.manaCost;

    protected Vector2 direction;
    protected LayerMask enemyLayer;

    public virtual void Init(Vector2 dir, LayerMask layer)
    {
        direction = dir.normalized;
        enemyLayer = layer;

        RotateTowardsDirection();
        Destroy(gameObject, data.lifetime);
    }

    protected virtual void Update()
    {
        transform.position += (Vector3)(direction * data.speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        enemy.TakeDamage(data.damage);
        OnHitEnemy(enemy, other);
        SpawnHitEffect(other);

        Destroy(gameObject);
    }

    protected abstract void OnHitEnemy(Enemy enemy, Collider2D other);

    protected virtual void SpawnHitEffect(Collider2D other)
    {
        if (data.hitEffectPrefab == null) return;
        Vector2 hitPoint = other.ClosestPoint(transform.position);
        GameObject fx = Instantiate(data.hitEffectPrefab, hitPoint, transform.rotation);
        Destroy(fx, data.hitEffectLifetime);
    }

    protected virtual void RotateTowardsDirection()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}