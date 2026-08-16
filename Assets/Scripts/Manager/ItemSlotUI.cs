using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ContainerType { Inventory, Hotbar, Equipment }

// A poser sur le GameObject "Icon" de chaque slot (Inventory, Hotbar, Equipment).
// Le script détecte automatiquement son index (= sibling index de son parent "Slot"),
// mais vous devez choisir manuellement le ContainerType dans l'inspecteur
// (astuce : sélectionnez tous les slots d'un même groupe et changez le champ en une fois).
[RequireComponent(typeof(Image))]
public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [Header("Slot Config")]
    public ContainerType containerType;

    private int slotIndex;
    private Image icon;
    private Canvas rootCanvas;

    // Icône flottante utilisée pendant le drag (une seule instance à la fois)
    private static GameObject dragIconObject;

    private IItemContainer Container
    {
        get
        {
            switch (containerType)
            {
                case ContainerType.Inventory: return InventoryManager.instance;
                case ContainerType.Hotbar: return HotbarManager.instance;
                case ContainerType.Equipment: return EquipmentManager.instance;
                default: return null;
            }
        }
    }

    private void Awake()
    {
        icon = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        // Le slot parent (le GameObject "Slot", "Slot (1)", etc.) donne l'index dans le tableau
        slotIndex = transform.parent.GetSiblingIndex();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Item item = Container?.GetItem(slotIndex);
        if (item == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        dragIconObject = new GameObject("DragIcon");
        dragIconObject.transform.SetParent(rootCanvas.transform, false);
        dragIconObject.transform.SetAsLastSibling();

        Image dragImage = dragIconObject.AddComponent<Image>();
        dragImage.sprite = icon.sprite;
        dragImage.raycastTarget = false;

        RectTransform dragRect = dragIconObject.GetComponent<RectTransform>();
        dragRect.anchorMin = new Vector2(0.5f, 0.5f);
        dragRect.anchorMax = new Vector2(0.5f, 0.5f);
        dragRect.pivot = new Vector2(0.5f, 0.5f);
        dragRect.sizeDelta = ((RectTransform)icon.transform).rect.size;

        CanvasGroup cg = dragIconObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0.85f;

        icon.color = new Color(1f, 1f, 1f, 0.35f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
        {
            RectTransform canvasRect = (RectTransform)rootCanvas.transform;
            Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : rootCanvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, cam, out Vector2 localPoint))
            {
                dragIconObject.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        icon.color = Color.white;

        if (dragIconObject != null)
        {
            Destroy(dragIconObject);
            dragIconObject = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI sourceSlot = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<ItemSlotUI>()
            : null;

        if (sourceSlot == null || sourceSlot == this) return;

        IItemContainer sourceContainer = sourceSlot.Container;
        IItemContainer targetContainer = Container;
        if (sourceContainer == null || targetContainer == null) return;

        Item sourceItem = sourceContainer.GetItem(sourceSlot.slotIndex);
        if (sourceItem == null) return;

        // Un slot d'équipement ne doit accepter qu'un EquipmentItem
        if (containerType == ContainerType.Equipment && !(sourceItem is EquipmentItem)) return;

        Item targetItem = targetContainer.GetItem(slotIndex);

        bool sourceWasEquipped = sourceSlot.containerType == ContainerType.Equipment;
        bool sourceWillBeEquipped = containerType == ContainerType.Equipment;

        // Échange des deux objets entre les deux conteneurs (peut être le même)
        sourceContainer.SetItem(sourceSlot.slotIndex, targetItem);
        targetContainer.SetItem(slotIndex, sourceItem);

        // sourceItem change d'état d'équipement ?
        if (sourceWasEquipped && !sourceWillBeEquipped)
            (sourceItem as EquipmentItem)?.NotifyUnequipped();
        else if (!sourceWasEquipped && sourceWillBeEquipped)
            (sourceItem as EquipmentItem)?.NotifyEquipped();

        // targetItem (qui part dans l'autre sens) change d'état aussi ?
        if (targetItem != null)
        {
            bool targetWasEquipped = containerType == ContainerType.Equipment;
            bool targetWillBeEquipped = sourceSlot.containerType == ContainerType.Equipment;

            if (targetWasEquipped && !targetWillBeEquipped)
                (targetItem as EquipmentItem)?.NotifyUnequipped();
            else if (!targetWasEquipped && targetWillBeEquipped)
                (targetItem as EquipmentItem)?.NotifyEquipped();
        }

        sourceContainer.RefreshUI();
        targetContainer.RefreshUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Item item = Container?.GetItem(slotIndex);
        if (item == null) return;

        if (containerType == ContainerType.Equipment)
        {
            (item as EquipmentItem)?.UnequipItem();
        }
        else
        {
            item.UseItem();
        }

        Container.RefreshUI();
    }
}
