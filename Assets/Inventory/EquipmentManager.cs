using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    public ItemData equippedWeapon;
    public ItemData equippedArmor;

    public event Action OnEquipmentChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Equip(ItemData item, int fromInventorySlotIndex)
    {
        if (item.equipSlot == EquipSlotType.None) return;

        ItemData previousItem = null;

        if (item.equipSlot == EquipSlotType.Weapon)
        {
            previousItem = equippedWeapon;
            equippedWeapon = item;
        }
        else if (item.equipSlot == EquipSlotType.Armor)
        {
            previousItem = equippedArmor;
            equippedArmor = item;
        }

        InventoryManager.Instance.RemoveItem(fromInventorySlotIndex, 1);

        if (previousItem != null)
        {
            InventoryManager.Instance.AddItem(previousItem, 1);
        }

        OnEquipmentChanged?.Invoke();
    }

    public void Unequip(EquipSlotType slotType)
    {
        ItemData item = slotType == EquipSlotType.Weapon ? equippedWeapon : equippedArmor;
        if (item == null) return;

        bool added = InventoryManager.Instance.AddItem(item, 1);
        if (!added) return; 

        if (slotType == EquipSlotType.Weapon) equippedWeapon = null;
        else if (slotType == EquipSlotType.Armor) equippedArmor = null;

        OnEquipmentChanged?.Invoke();
    }

    public int GetTotalAttackBonus() => equippedWeapon != null ? equippedWeapon.attackBonus : 0;
    public int GetTotalDefenseBonus() => equippedArmor != null ? equippedArmor.defenseBonus : 0;
}