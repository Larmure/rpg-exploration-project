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


    public override void UseItem()
    {
        base.UseItem();
        switch (foodType)
        {
            case FoodType.health:
                PlayerScript.instance.currentHealth += amount;
                break;
        }
        RemoveItemFromInventory();
        Inventory.instance.LoadInventory();
    }

}
