using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    food, ressource
}

public class Item : MonoBehaviour
{

    public ItemType itemType;

    public Sprite icon;
    public string nameItem;
    public int amount;
    public bool isUsed;

    public virtual void UseItem()
    {
        Debug.Log($"Using {nameItem}");
    }

    public virtual void AddItemToInventory()
    {
        var haveFoundItem = false;

        for (int i = 0; i < Inventory.instance.inventories.Length; i++)
        {
            if(Inventory.instance.inventories[i] != null)
            {
                if(Inventory.instance.inventories[i].nameItem == this.nameItem)
                {
                    if(Inventory.instance.inventories[i].amount < 99)
                    {
                        haveFoundItem = true;
                        Inventory.instance.inventories[i].amount += amount;
                        break;
                    }
                }
            }
        }

        if(haveFoundItem == false)
        {
            for (int i = 0; i < Inventory.instance.inventories.Length; i++)
            {
                if(Inventory.instance.inventories[i] == null)
                {
                    Inventory.instance.inventories[i] = this;
                    break;
                }
            }
        }

        Inventory.instance.LoadInventory();

        if (haveFoundItem) Destroy(gameObject);
    }

    public virtual void RemoveItemFromInventory()
    {
        for (int i = 0; i < Inventory.instance.inventories.Length; i++)
        {
            if(Inventory.instance.inventories[i] != null)
            {
                if(Inventory.instance.inventories[i].nameItem == this.nameItem)
                {
                    Inventory.instance.inventories[i].amount -= amount;

                    if(Inventory.instance.inventories[i].amount <= 0)
                    {
                        Inventory.instance.inventories[i] = null;
                        Inventory.instance.LoadInventory();
                        Destroy(gameObject);
                    }

                    break;
                }
            }
        }
        Inventory.instance.LoadInventory();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (isUsed)
            {
                return;
            }

            GetComponent<SpriteRenderer>().enabled = false;

            foreach (var item in GetComponents<BoxCollider2D>())
            {
                item.isTrigger = true;
            }

            isUsed = true;

            AddItemToInventory();

            Inventory.instance.LoadInventory();
        }
    }

}
