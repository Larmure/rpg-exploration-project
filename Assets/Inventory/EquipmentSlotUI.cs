using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    public EquipSlotType slotType;

    void OnEnable()
    {
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.OnEquipmentChanged += Refresh;
    }

    void Start()
    {
        Refresh();
    }

    void OnDisable()
    {
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.OnEquipmentChanged -= Refresh;
    }

    private ItemData GetEquippedItem()
    {
        if (EquipmentManager.Instance == null) return null;

        return slotType == EquipSlotType.Weapon
            ? EquipmentManager.Instance.equippedWeapon
            : EquipmentManager.Instance.equippedArmor;
    }

    public void Refresh()
    {
        ItemData item = GetEquippedItem();

        if (item != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = item.icon;
            iconImage.color = Color.white; 
        }
        else
        {
            iconImage.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemData item = GetEquippedItem();
        if (item != null) ItemTooltip.Instance.Show(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemData item = GetEquippedItem();
        if (item == null) return;

        DragIcon.Instance.Show(item.icon);
        iconImage.enabled = false;
        ItemTooltip.Instance.Hide();
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragIcon.Instance.Hide();
        Refresh();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        InventorySlotUI droppedSlot = droppedObject?.GetComponent<InventorySlotUI>();
        if (droppedSlot == null) return;

        InventorySlot slotData = InventoryManager.Instance.slots[droppedSlot.slotIndex];
        if (slotData.IsEmpty || slotData.item.equipSlot != slotType) return;

        EquipmentManager.Instance.Equip(slotData.item, droppedSlot.slotIndex);
    }
}