using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;
    [SerializeField] private TextMeshProUGUI choiceText1;
    [SerializeField] private TextMeshProUGUI choiceText2;

    private string[] currentLines;
    private string[] currentChoices;
    private System.Action<int> onChoiceMade;

    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isChoiceActive = false;

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

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        if (choiceButton1 != null)
            choiceButton1.onClick.AddListener(() => SelectChoice(0));

        if (choiceButton2 != null)
            choiceButton2.onClick.AddListener(() => SelectChoice(1));
    }

    private void Update()
    {
        if (!isDialogueActive || isChoiceActive) return;

        // Clic gauche ou Espace pour avancer, sans que ça déclenche une attaque
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    public bool IsDialogueActive() => isDialogueActive || isChoiceActive;

    /// <summary>
    /// Démarre un dialogue. Si "choices" est fourni, à la fin des lignes,
    /// des options cliquables apparaissent et "onChoiceSelected" est appelé
    /// avec l'index choisi (0 ou 1) au lieu de fermer directement le dialogue.
    /// </summary>
    public void StartDialogue(string speakerName, string[] lines, string[] choices = null, System.Action<int> onChoiceSelected = null)
    {
        if (lines == null || lines.Length == 0) return;

        if (dialoguePanel == null || speakerNameText == null || dialogueText == null)
        {
            Debug.LogError("[DialogueManager] Références UI manquantes dans l'Inspector !");
            return;
        }

        currentLines = lines;
        currentChoices = choices;
        onChoiceMade = onChoiceSelected;
        currentLineIndex = 0;
        isDialogueActive = true;
        isChoiceActive = false;

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        dialoguePanel.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        speakerNameText.text = speakerName;
        dialogueText.text = currentLines[currentLineIndex];
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            if (currentChoices != null && currentChoices.Length > 0)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        dialogueText.text = currentLines[currentLineIndex];
    }

    private void ShowChoices()
    {
        isChoiceActive = true;

        if (choicesContainer == null || choiceButton1 == null || choiceText1 == null)
        {
            Debug.LogError("[DialogueManager] Références Choices UI manquantes dans l'Inspector !");
            EndDialogue();
            return;
        }

        dialogueText.gameObject.SetActive(false);
        choicesContainer.SetActive(true);

        choiceText1.text = currentChoices[0];
        choiceButton1.gameObject.SetActive(true);

        bool hasSecondChoice = currentChoices.Length > 1;
        if (choiceButton2 != null)
        {
            choiceButton2.gameObject.SetActive(hasSecondChoice);
            if (hasSecondChoice) choiceText2.text = currentChoices[1];
        }
    }

    private void SelectChoice(int index)
    {
        if (!isChoiceActive) return;
        if (currentChoices == null || index >= currentChoices.Length) return;

        var callback = onChoiceMade;

        isChoiceActive = false;
        isDialogueActive = false;

        if (choicesContainer != null)
            choicesContainer.SetActive(false);

        dialoguePanel.SetActive(false);

        callback?.Invoke(index);
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        isChoiceActive = false;
        dialoguePanel.SetActive(false);
    }
}
