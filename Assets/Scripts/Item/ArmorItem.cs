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
        if (Player.instance != null)
        {
            Player.instance.SetArmorPoints(Player.instance.GetArmorPoints() + armorPoints);
            Debug.Log($"{nameItem} : +{armorPoints} armor (total: {Player.instance.GetArmorPoints()})");
        }
    }

    protected override void OnUnequipped()
    {
        if (Player.instance != null)
        {
            Player.instance.SetArmorPoints(Player.instance.GetArmorPoints() - armorPoints);
            Debug.Log($"{nameItem}  : -{armorPoints} armor (total: {Player.instance.GetArmorPoints()})");
        }
    }
}