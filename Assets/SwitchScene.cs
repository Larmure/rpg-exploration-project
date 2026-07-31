using UnityEngine;
using UnityEngine.SceneManagement;


public class SwitchScene : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;
    [SerializeField] private string targetSpawnPointId; // pour savoir où apparaître dans la nouvelle scène

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TransitionManager.Instance.LoadScene(sceneToLoad, targetSpawnPointId);
        }
    }
}
