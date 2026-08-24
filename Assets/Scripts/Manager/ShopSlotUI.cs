using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A poser sur chaque slot de la grille ShopDisplay (le même prefab de slot
/// que l'inventaire). Le prix n'est plus affiché ici : il est géré par un
/// tooltip unique dans ShopManager qui suit la souris.
/// </summary>
public class ShopSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int slotIndex;
    private ShopManager shopManager;

    public void Setup(int index, ShopManager manager)
    {
        slotIndex = index;
        shopManager = manager;
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
