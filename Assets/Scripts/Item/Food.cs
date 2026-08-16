using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum FoodType
{
    health
}
public class Food : Item
{
    public FoodType foodType;
    protected override void Awake()
    {
        base.Awake();
        ItemType = ItemType.food;
    }

    public override void UseItem()
    {
        base.UseItem();
        switch (foodType)
        {
            case FoodType.health:
                {
                    if(PlayerScript.instance.currentHealth + amount <= PlayerScript.instance.maxHealth)
                    {
                        PlayerScript.instance.Heal(amount);
                        RemoveItem();
                    }
                }
                break;
        }
        InventoryManager.instance.LoadInventory();
        HotbarManager.instance.LoadHotbar();
    }

}
