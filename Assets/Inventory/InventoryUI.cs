using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GameObject slotPrefab;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    void Start()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshAllSlots;
        BuildSlots();
        RefreshAllSlots();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshAllSlots;
    }

    private void BuildSlots()
    {
        for (int i = 0; i < InventoryManager.Instance.slots.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUI.slotIndex = i;
            slotUIs.Add(slotUI);
        }
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            slotUIs[i].Refresh(InventoryManager.Instance.slots[i]);
        }
    }
}