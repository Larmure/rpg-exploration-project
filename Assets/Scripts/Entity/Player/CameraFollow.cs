using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 boundsMin;
    [SerializeField] private Vector2 boundsMax; 

    private Vector3 velocity = Vector3.zero;
    private float initialZ;
    private Camera cam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Une caméra existe déjà (venant d'une scène précédente) :
            // on détruit ce doublon pour garder celle qui a l'état à jour.
            Destroy(gameObject);
            return; // évite d'exécuter Start()/LateUpdate() sur l'objet détruit
        }
    }

    private void Start()
    {
        initialZ = transform.position.z;
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, initialZ);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        if (useBounds && cam != null)
        {
            smoothedPosition = ClampToBounds(smoothedPosition);
        }

        transform.position = smoothedPosition;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        float minX = boundsMin.x + camHalfWidth;
        float maxX = boundsMax.x - camHalfWidth;
        float minY = boundsMin.y + camHalfHeight;
        float maxY = boundsMax.y - camHalfHeight;

        if (minX > maxX || minY > maxY)
        {
            Debug.LogWarning($"[CameraFollow] Bounds trop petites ! camHalfWidth={camHalfWidth}, camHalfHeight={camHalfHeight}, boundsMin={boundsMin}, boundsMax={boundsMax}");
        }

        float clampedX = minX <= maxX ? Mathf.Clamp(position.x, minX, maxX) : (boundsMin.x + boundsMax.x) * 0.5f;
        float clampedY = minY <= maxY ? Mathf.Clamp(position.y, minY, maxY) : (boundsMin.y + boundsMax.y) * 0.5f;

        return new Vector3(clampedX, clampedY, position.z);
    }

    /// <summary>
    /// À appeler quand une nouvelle scène charge un nouveau joueur/target
    /// (puisque l'objet target d'origine a été détruit avec l'ancienne scène).
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        boundsMin = min;
        boundsMax = max;
    }

    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((boundsMin.x + boundsMax.x) * 0.5f, (boundsMin.y + boundsMax.y) * 0.5f, 0f);
        Vector3 size = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }

    
}