using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    food, armor, coin
}

public class Item : MonoBehaviour
{
    public ItemType ItemType;

    public Sprite icon;
    public string nameItem;
    public int amount;
    public bool isUsed;
    public int price;

    protected virtual void Awake() { }

    public virtual void UseItem()
    {
        Debug.Log($"Using {nameItem}");
    }

    public virtual bool AddItemToHotbar()
    {
        return TryAddItemToArray(HotbarManager.instance.inventories, HotbarManager.instance.LoadHotbar);
    }

    public virtual bool AddItemToInventory()
    {
        if (InventoryManager.instance == null) return false;
        return TryAddItemToArray(InventoryManager.instance.inventories, InventoryManager.instance.LoadInventory);
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

                    // L'objet doit survivre au changement de scène, comme le
                    // manager qui le référence.
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);

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

    public virtual bool RemoveItemFromHotbar(int quantity = 1)
    {
        return TryRemoveItemFromArray(HotbarManager.instance.inventories, HotbarManager.instance.LoadHotbar, quantity);
    }

    public virtual bool RemoveItemFromInventory(int quantity = 1)
    {
        if (InventoryManager.instance == null) return false;
        return TryRemoveItemFromArray(InventoryManager.instance.inventories, InventoryManager.instance.LoadInventory, quantity);
    }

    public virtual void RemoveItem(int quantity = 1)
    {
        if (!RemoveItemFromHotbar(quantity))
        {
            RemoveItemFromInventory(quantity);
        }
    }   

    private bool TryRemoveItemFromArray(Item[] items, System.Action reload, int quantity)
    {
        bool found = false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].nameItem == this.nameItem)
            {
                found = true;

                items[i].amount -= quantity;

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
        HotbarManager.instance.LoadHotbar();
    }
}


