using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;

    public int slotIndex;

    public void Refresh(InventorySlot slotData)
    {
        if (slotData == null || slotData.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slotData.item.icon;
            quantityText.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
        }
    }

    private InventorySlot GetSlotData()
    {
        return InventoryManager.Instance.slots[slotIndex];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventorySlot slot = GetSlotData();
        if (!slot.IsEmpty)
        {
            ItemTooltip.Instance.Show(slot.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySlot slot = GetSlotData();
        if (slot.IsEmpty) return;

        DragIcon.Instance.Show(slot.item.icon);
        iconImage.enabled = false;
        ItemTooltip.Instance.Hide();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // The position is handled directly in DragIcon.Update()
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragIcon.Instance.Hide();
        Refresh(GetSlotData());
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        InventorySlotUI droppedSlot = droppedObject.GetComponent<InventorySlotUI>();
        if (droppedSlot != null)
        {
            InventoryManager.Instance.SwapSlots(droppedSlot.slotIndex, slotIndex);
            return;
        }

        EquipmentSlotUI equipSlot = droppedObject.GetComponent<EquipmentSlotUI>();
        if (equipSlot != null)
        {
            EquipmentManager.Instance.Unequip(equipSlot.slotType);
        }
    }
}