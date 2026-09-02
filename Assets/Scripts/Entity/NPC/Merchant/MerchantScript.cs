using UnityEngine;

public class Merchant : NPC
{
    [Header("Shop")]
    [Tooltip("Les items que ce marchand vend (prefabs d'Item).")]
    public Item[] itemsForSale;

    protected override void OnDialogueEnd()
    {
        OpenShop();
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

/*
"Hey Pal!",
        "Need something ?"
        */