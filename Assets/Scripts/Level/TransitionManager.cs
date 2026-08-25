using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    private string pendingSpawnId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // détache l'objet pour qu'il devienne racine
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, string spawnId)
    {
        pendingSpawnId = spawnId;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    SceneManager.sceneLoaded -= OnSceneLoaded;

    Debug.Log($"[Transition] Scène chargée : {scene.name}, spawnId recherché : '{pendingSpawnId}'");

    SpawnPoint[] points = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
    Debug.Log($"[Transition] {points.Length} SpawnPoint(s) trouvé(s) dans la scène.");

    bool found = false;

    foreach (var point in points)
    {
        Debug.Log($"[Transition] SpawnPoint trouvé avec id='{point.id}' à la position {point.transform.position}");

        if (point.id == pendingSpawnId)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log($"[Transition] {allPlayers.Length} objet(s) taggé(s) 'Player' trouvé(s).");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log($"[Transition] Position AVANT : {player.transform.position}");

                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.position = point.transform.position;
                }
                player.transform.position = point.transform.position;

                Debug.Log($"[Transition] Position APRÈS : {player.transform.position}");

                found = true;
            }
            break;
        }
    }

    if (!found)
    {
        Debug.LogWarning($"TransitionManager : aucun SpawnPoint avec l'id '{pendingSpawnId}' trouvé dans '{scene.name}'.");
    }
}
}