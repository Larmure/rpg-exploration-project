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

    public virtual bool AddItemToHotbar()
    {
        return TryAddItemToArray(Hotbar.instance.inventories, Hotbar.instance.LoadHotbar);
    }

    public virtual bool AddItemToInventory()
    {
        if (Inventory.instance == null) return false;
        return TryAddItemToArray(Inventory.instance.inventories, Inventory.instance.LoadInventory);
    }

    private bool TryAddItemToArray(Item[] items, System.Action reload)
    {
        var haveFoundItem = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null &&
                items[i].nameItem == this.nameItem &&
               items[i].amount < 99)
            {
                haveFoundItem = true;
                items[i].amount += amount;
                break;
            }
        }

        if (!haveFoundItem)
        {
            bool foundEmptySlot = false;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = this;
                    foundEmptySlot = true;
                    break;
                }
            }

            if (!foundEmptySlot)
            {
                return false;
            }
        }

        reload();

        if (haveFoundItem) Destroy(gameObject);

        return true;
    }

    public virtual bool RemoveItemFromHotbar()
    {
        return TryRemoveItemFromArray(Hotbar.instance.inventories, Hotbar.instance.LoadHotbar);
    }

    public virtual bool RemoveItemFromInventory()
    {
        if (Inventory.instance == null) return false;
        return TryRemoveItemFromArray(Inventory.instance.inventories, Inventory.instance.LoadInventory);
    }

    public virtual void RemoveItem()
    {
    if (!RemoveItemFromHotbar())
    {
        RemoveItemFromInventory();
    }
}   

    private bool TryRemoveItemFromArray(Item[] items, System.Action reload)
    {
        bool found = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].nameItem == this.nameItem)
            {
                found = true;

                items[i].amount -= amount;

                if (items[i].amount <= 0)
                {
                    items[i] = null;
                    reload();
                    Destroy(gameObject);
                }

                break;
            }
        }

        reload();
        return found;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isUsed) return;

            GetComponent<SpriteRenderer>().enabled = false;

            foreach (var item in GetComponents<BoxCollider2D>())
            {
                item.isTrigger = true;
            }

            isUsed = true;

            bool addedToHotbar = AddItemToHotbar();

            if (!addedToHotbar)
            {
                bool addedToInventory = AddItemToInventory();

                if (!addedToInventory)
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    foreach (var item in GetComponents<BoxCollider2D>())
                    {
                        item.isTrigger = false;
                    }
                    isUsed = false;
                    return;
                }
            }
        }

        Hotbar.instance.LoadHotbar();
    }
}


