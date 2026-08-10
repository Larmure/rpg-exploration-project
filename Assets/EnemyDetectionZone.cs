using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    private EnemyScript enemyScript;

    void Start()
    {
        enemyScript = GetComponentInParent<EnemyScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyScript.PlayerDetected();
        }
    }
}