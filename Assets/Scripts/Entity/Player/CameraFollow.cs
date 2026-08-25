using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;
    private float initialZ;

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
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, initialZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    /// <summary>
    /// À appeler quand une nouvelle scène charge un nouveau joueur/target
    /// (puisque l'objet target d'origine a été détruit avec l'ancienne scène).
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}