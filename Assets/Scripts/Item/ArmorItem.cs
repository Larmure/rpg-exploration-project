using UnityEngine;

public class ArmorItem : EquipmentItem
{
    [Header("Armor")]
    public int armorPoints;

    protected override void Awake()
    {
        base.Awake();
        ItemType = ItemType.armor;
    }

    protected override void OnEquipped()
    {
        if (PlayerScript.instance != null)
        {
            PlayerScript.instance.armorPoints += armorPoints;
            Debug.Log($"{nameItem} : +{armorPoints} armor (total: {PlayerScript.instance.armorPoints})");
        }
    }

    protected override void OnUnequipped()
    {
        if (PlayerScript.instance != null)
        {
            PlayerScript.instance.armorPoints -= armorPoints;
            Debug.Log($"{nameItem}  : -{armorPoints} armor (total: {PlayerScript.instance.armorPoints})");
        }
    }
}