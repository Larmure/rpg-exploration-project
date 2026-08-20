using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private void Awake()
    {
        instance = this;
    }

    // TODO étape 2 : ouvrir le panel Shop (UI façon inventaire, cadre vert)
    public void OpenBuyMenu(Merchant merchant)
    {
        Debug.Log($"[ShopManager] OpenBuyMenu appelé pour {merchant.name} (à implémenter étape 2/3).");
    }

    // TODO étape 4 : ouvrir l'inventaire du joueur en mode vente
    public void OpenSellMenu()
    {
        Debug.Log("[ShopManager] OpenSellMenu appelé (à implémenter étape 4).");
    }
}
