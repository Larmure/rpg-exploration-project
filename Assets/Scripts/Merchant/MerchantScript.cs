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

    [Header("Shop")]
    [Tooltip("Les items que ce marchand vend (prefabs d'Item).")]
    public Item[] itemsForSale;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(
            merchantName,
            dialogueLines,
            null,
            null,
            OpenShop
        );
    }

    private void OpenShop()
    {
        if (ShopManager.instance == null)
        {
            Debug.LogError("[Merchant] ShopManager introuvable dans la scène.");
            return;
        }

        ShopManager.instance.OpenShop(this);
    }
}