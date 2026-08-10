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

    public virtual void AddItemToHotbar()
    {
        var haveFoundItem = false;

        for (int i = 0; i < Hotbar.instance.inventories.Length; i++)
        {
            if(Hotbar.instance.inventories[i] != null)
            {
                if(Hotbar.instance.inventories[i].nameItem == this.nameItem)
                {
                    if(Hotbar.instance.inventories[i].amount < 99)
                    {
                        haveFoundItem = true;
                        Hotbar.instance.inventories[i].amount += amount;
                        break;
                    }
                }
            }
        }

        if(haveFoundItem == false)
        {
            for (int i = 0; i < Hotbar.instance.inventories.Length; i++)
            {
                if(Hotbar.instance.inventories[i] == null)
                {
                    Hotbar.instance.inventories[i] = this;
                    break;
                }
            }
        }

        Hotbar.instance.LoadHotbar();

        if (haveFoundItem) Destroy(gameObject);
    }

    public virtual void RemoveItemFromHotbar()
    {
        for (int i = 0; i < Hotbar.instance.inventories.Length; i++)
        {
            if(Hotbar.instance.inventories[i] != null)
            {
                if(Hotbar.instance.inventories[i].nameItem == this.nameItem)
                {
                    Hotbar.instance.inventories[i].amount -= amount;

                    if(Hotbar.instance.inventories[i].amount <= 0)
                    {
                        Hotbar.instance.inventories[i] = null;
                        Hotbar.instance.LoadHotbar();
                        Destroy(gameObject);
                    }

                    break;
                }
            }
        }
        Hotbar.instance.LoadHotbar();
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

            AddItemToHotbar();

            Hotbar.instance.LoadHotbar();
        }
    }

}
