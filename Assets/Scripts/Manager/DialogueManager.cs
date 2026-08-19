using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        // Clic gauche ou Espace pour avancer, sans que ça déclenche une attaque
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    public bool IsDialogueActive() => isDialogueActive;

    public void StartDialogue(string speakerName, string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        if (dialoguePanel == null || speakerNameText == null || dialogueText == null)
        {
            Debug.LogError("[DialogueManager] Références UI manquantes dans l'Inspector !");
            return; // on n'active pas isDialogueActive si on ne peut pas afficher le dialogue
        }

        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        speakerNameText.text = speakerName;
        dialogueText.text = currentLines[currentLineIndex];
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = currentLines[currentLineIndex];
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}