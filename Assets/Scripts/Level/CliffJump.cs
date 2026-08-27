using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CliffJump : MonoBehaviour
{
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private float directionThreshold = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector2 landingCheckSize = new Vector2(0.4f, 0.4f);

    private Rigidbody2D rb;
    private Collider2D col;
    private Player player;
    private Animator anim;
    private bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isJumping) return;
        if (((1 << other.gameObject.layer) & ledgeLayer) == 0) return;

        LedgeZone ledge = other.GetComponent<LedgeZone>();
        if (ledge == null) return;

        Vector2 moveDir = player != null ? player.GetLastMoveDirection() : Vector2.zero;
        Vector2 ledgeDir = ledge.GetDirectionVector();

        if (Vector2.Dot(moveDir.normalized, ledgeDir) < directionThreshold) return;

        Vector2 start = rb.position;
        Vector2 end = start + ledge.GetDirectionVector() * ledge.jumpDistance;

        if (!IsLandingClear(end))
        {
            Debug.Log($"[CliffJump] Atterrissage bloqué en {end}");
            return;
        }

        StartCoroutine(DoJump(ledge, start, end));
    }

    private bool IsLandingClear(Vector2 landingPos)
    {
        Collider2D hit = Physics2D.OverlapBox(landingPos, landingCheckSize, 0f, obstacleLayer);
        return hit == null;
    }

    private IEnumerator DoJump(LedgeZone ledge, Vector2 start, Vector2 end)
    {
        isJumping = true;
        if (player != null) player.SetControlLocked(true);
        
        // 1. On lance l'animation
        if (anim != null) anim.SetBool("IsJumping", true);

        col.enabled = false;

        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, ledge.jumpDuration);
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rb.MovePosition(Vector2.Lerp(start, end, t));
            
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(end);
        yield return new WaitForFixedUpdate();

        col.enabled = true;
        
        // 2. On arrête l'animation à l'atterrissage
        if (anim != null) anim.SetBool("IsJumping", false);

        if (player != null) player.SetControlLocked(false);
        isJumping = false;
    }
}