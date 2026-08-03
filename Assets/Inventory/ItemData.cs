using UnityEngine;

public enum ItemType
{
    Consumable,
    Weapon,
    Armor,
    Misc
}

public enum EquipSlotType
{
    None,
    Weapon,
    Armor
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Type")]
    public ItemType itemType;
    public EquipSlotType equipSlot = EquipSlotType.None; 

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Stats (if equipment)")]
    public int attackBonus = 0;
    public int defenseBonus = 0;

    [Header("Stats (if consumable)")]
    public int healAmount = 0;
}