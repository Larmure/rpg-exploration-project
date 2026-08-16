using UnityEngine;

public class EquipmentItem : Item
{
    public override void UseItem()
    {
        base.UseItem();

        if (EquipmentManager.instance == null)
        {
            Debug.LogWarning("EquipmentManager not found.");
            return;
        }

        Item[] equipments = EquipmentManager.instance.equipments;

        for (int i = 0; i < equipments.Length; i++)
        {
            if (equipments[i] == null)
            {
                RemoveFromCurrentContainer();

                equipments[i] = this;

                EquipmentManager.instance.LoadEquipment();
                HotbarManager.instance.LoadHotbar();
                OnEquipped();
                return;
            }
        }

        Debug.Log($"No slot available for {nameItem}.");
    }

    public void UnequipItem()
    {
        if (EquipmentManager.instance == null) return;

        Item[] equipments = EquipmentManager.instance.equipments;
        int mySlot = System.Array.IndexOf(equipments, this);

        if (mySlot == -1) return; 

        bool added = AddItemToHotbar();

        if (!added)
        {
            added = AddItemToInventory();
        }

        if (!added)
        {
            Debug.Log($"No room to unequip {nameItem}");
            return; 
        }

        equipments[mySlot] = null;
        EquipmentManager.instance.LoadEquipment();
        HotbarManager.instance.LoadHotbar();
        OnUnequipped();
    }

    private void RemoveFromCurrentContainer()
    {
        Item[] hotbar = HotbarManager.instance.inventories;
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i] == this)
            {
                hotbar[i] = null;
                HotbarManager.instance.LoadHotbar();
                return;
            }
        }

        if (InventoryManager.instance != null)
        {
            Item[] inventory = InventoryManager.instance.inventories;
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == this)
                {
                    inventory[i] = null;
                    InventoryManager.instance.LoadInventory();
                    return;
                }
            }
        }
    }

    public void NotifyEquipped() => OnEquipped();
    public void NotifyUnequipped() => OnUnequipped();

    protected virtual void OnEquipped() { }
    protected virtual void OnUnequipped() { }
}