using UnityEngine;

// À accrocher sur chaque petit GameObject "zone de saut" placé pile sur
// une tuile de bord de falaise. Le Collider2D dessus doit être en "Is Trigger",
// et le GameObject doit être sur le Layer "Ledge".
[RequireComponent(typeof(Collider2D))]
public class LedgeZone : MonoBehaviour
{
    public enum JumpDirection { Up, Down, Left, Right }

    [Tooltip("Direction dans laquelle le joueur doit se déplacer pour déclencher le saut")]
    public JumpDirection direction = JumpDirection.Down;

    [Tooltip("Distance parcourue pendant le saut (en unités du monde, ex: 1.5 pour sauter 1 tuile et demie)")]
    public float jumpDistance = 1.5f;

    [Tooltip("Durée du saut en secondes")]
    public float jumpDuration = 0.35f;

    public Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case JumpDirection.Up: return Vector2.up;
            case JumpDirection.Down: return Vector2.down;
            case JumpDirection.Left: return Vector2.left;
            case JumpDirection.Right: return Vector2.right;
        }
        return Vector2.down;
    }

    // Petit gizmo pour visualiser la direction du saut dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 dir = (Vector3)(Vector2)GetDirectionVector();
        Gizmos.DrawLine(transform.position, transform.position + dir * jumpDistance);
        Gizmos.DrawWireSphere(transform.position + dir * jumpDistance, 0.1f);
    }
}
