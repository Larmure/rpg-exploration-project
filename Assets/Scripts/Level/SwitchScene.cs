using UnityEngine;
using UnityEngine.SceneManagement;


public class SwitchScene : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;
    [SerializeField] private string targetSpawnPointId; // pour savoir où apparaître dans la nouvelle scène

    [Header("Story Requirement")]
    [Tooltip("If set, the player must have talked to this NPC before being able to switch scenes.")]
    [SerializeField] private string requiredNpcId;
    [SerializeField] private string blockedMessage = "I should talk to the someone.";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(requiredNpcId) &&
            (StoryState.Instance == null || !StoryState.Instance.HasTalkedToNPC(requiredNpcId)))
        {
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.StartDialogue("Me", new string[] { blockedMessage });
            }
            return;
        }

        TransitionManager.Instance.LoadScene(sceneToLoad, targetSpawnPointId);
    }
}
