using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    private Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.PlayerDetected();
        }
    }
}