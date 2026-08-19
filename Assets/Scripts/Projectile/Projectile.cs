using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitEffectLifetime = 1f;
    [SerializeField] private bool orientEffectToNormal = true;

    private Vector2 direction;
    private LayerMask enemyLayer;

    public void Init(Vector2 dir, int dmg, LayerMask layer)
    {
        direction = dir.normalized;
        damage = dmg;
        enemyLayer = layer;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        EnemyScript enemy = other.GetComponentInParent<EnemyScript>();
        if (enemy != null)
        {
            Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockDir, knockbackForce, knockbackDuration);
            enemy.TakeDamage(damage);

            SpawnHitEffect(other);

            if (destroyOnHit)
                Destroy(gameObject);
        }
    }

    private void SpawnHitEffect(Collider2D other)
    {
        if (hitEffectPrefab == null) return;

        Vector2 hitPoint = other.ClosestPoint(transform.position);

        Quaternion effectRotation = transform.rotation;

        if (orientEffectToNormal)
        {
            float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            effectRotation = Quaternion.Euler(0f, 0f, angle);
        }

        GameObject fx = Instantiate(hitEffectPrefab, hitPoint, effectRotation);
        Destroy(fx, hitEffectLifetime);
    }
}