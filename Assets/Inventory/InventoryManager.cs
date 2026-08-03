using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int inventorySize = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item && slot.quantity < item.maxStackSize)
                {
                    int spaceLeft = item.maxStackSize - slot.quantity;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.quantity += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        while (amount > 0)
        {
            InventorySlot emptySlot = slots.Find(s => s.IsEmpty);
            if (emptySlot == null)
            {
                Debug.Log("Inventory full !");
                OnInventoryChanged?.Invoke();
                return false;
            }

            int toAdd = item.isStackable ? Mathf.Min(item.maxStackSize, amount) : 1;
            emptySlot.item = item;
            emptySlot.quantity = toAdd;
            amount -= toAdd;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        InventorySlot slot = slots[slotIndex];
        if (slot.IsEmpty) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0)
        {
            slot.Clear();
        }

        OnInventoryChanged?.Invoke();
    }

    // For drag and drop 
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= slots.Count || indexB < 0 || indexB >= slots.Count) return;

        (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        OnInventoryChanged?.Invoke();
    }

    public void UseItem(int slotIndex)
    {
        InventorySlot slot = slots[slotIndex];
        if (slot.IsEmpty) return;

        if (slot.item.itemType == ItemType.Consumable)
        {
            // Only for heal, need to add more effects later...
            PlayerScript player = FindFirstObjectByType<PlayerScript>();
            if (player != null && slot.item.healAmount > 0)
            {
                player.Heal(slot.item.healAmount);
            }

            RemoveItem(slotIndex, 1);
        }
        else if (slot.item.itemType == ItemType.Weapon || slot.item.itemType == ItemType.Armor)
        {
            EquipmentManager.Instance.Equip(slot.item, slotIndex);
        }
    }
}