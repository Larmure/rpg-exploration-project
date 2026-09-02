using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraBoundsZone : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        ApplyBounds();
    }

    public void ApplyBounds()
    {
        Bounds bounds = boxCollider.bounds;

        Vector2 min = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 max = new Vector2(bounds.max.x, bounds.max.y);

        //Debug.Log($"[CameraBoundsZone] min={min}, max={max}, size={bounds.size}");

        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetBounds(min, max);
        }
        else
        {
            //Debug.LogWarning("[CameraBoundsZone] CameraFollow.Instance est null !");
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}