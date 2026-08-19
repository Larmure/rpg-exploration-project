using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] private string merchantName = "Merchant";

    [Header("Dialogue")]
    [SerializeField] private string[] dialogueLines = new string[]
    {
        "Hey Pal!", 
        "Need something ?"
    };

    public void Interact()
    {
        Debug.Log("Interaction avec le marchand !");
        DialogueManager.Instance.StartDialogue(merchantName, dialogueLines);

        // TODO: remplace par ton système de dialogue quand il existera
        // Exemple si tu as un DialogueManager :
        // DialogueManager.Instance.StartDialogue(dialogueLines);

        // TODO: ici tu pourras plus tard ouvrir l'UI du shop
        // ShopUI.Instance.Open();
    }
}