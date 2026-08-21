using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// A poser sur chaque slot de la grille ShopDisplay (le même prefab de slot
/// que l'inventaire, avec un enfant TMP "PriceText" en plus).
/// </summary>
public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI priceText;

    private int slotIndex;
    private ShopManager shopManager;

    public void Setup(int index, ShopManager manager)
    {
        slotIndex = index;
        shopManager = manager;
        HidePrice();
    }

    public void ShowPrice(string text, Color color)
    {
        if (priceText == null) return;
        priceText.text = text;
        priceText.color = color;
        priceText.gameObject.SetActive(true);
    }

    public void HidePrice()
    {
        if (priceText == null) return;
        priceText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopManager.ShowPriceForSlot(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopManager.HidePriceForSlot(slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        shopManager.OnSlotClicked(slotIndex);
    }
}
