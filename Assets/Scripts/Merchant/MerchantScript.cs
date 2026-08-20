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
        Debug.Log("Interaction avec le marchand !");
        DialogueManager.Instance.StartDialogue(
            merchantName,
            dialogueLines,
            new string[] { "Buy", "Sell" },
            OnMerchantChoice
        );
    }

    private void OnMerchantChoice(int choiceIndex)
    {
        if (ShopManager.instance == null)
        {
            Debug.LogError("[Merchant] ShopManager introuvable dans la scène.");
            return;
        }

        if (choiceIndex == 0)
        {
            ShopManager.instance.OpenBuyMenu(this);
        }
        else
        {
            ShopManager.instance.OpenSellMenu();
        }
    }
}
