using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour, IItemContainer
{
    public static InventoryManager instance;
    public Item[] inventories = new Item[24];
    public Transform inventoryDisplay;
    private bool isInventoryOpen = false;
    private Sprite blankItem;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        blankItem = inventoryDisplay.transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite;
        inventoryDisplay.gameObject.SetActive(false);
    }

    // --- IItemContainer ---

    public Item GetItem(int index)
    {
        if (index < 0 || index >= inventories.Length) return null;
        return inventories[index];
    }

    public void SetItem(int index, Item item)
    {
        if (index < 0 || index >= inventories.Length) return;
        inventories[index] = item;
    }

    public void RefreshUI()
    {
        LoadInventory();
    }

    // ----------------------

    public void LoadInventory()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            var icon = inventoryDisplay.transform.GetChild(i).transform.Find("Icon").GetComponent<Image>();
            var amountText = inventoryDisplay.transform.GetChild(i).transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            if (inventories[i] == null)
            {
                amountText.text = "";
                icon.sprite = blankItem;
                continue;
            }

            amountText.text = "" + inventories[i].amount;
            icon.sprite = inventories[i].icon;
        }
    }

    private void Update()
    {
        if (ShopManager.instance != null && ShopManager.instance.IsShopOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void SetSlot(int index)
    {
        foreach (Transform slot in inventoryDisplay)
        {
            slot.transform.Find("Outline").gameObject.SetActive(false);
        }
        inventoryDisplay.transform.GetChild(index).transform.Find("Outline").gameObject.SetActive(true);
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryDisplay.gameObject.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            LoadInventory();
        }
    }
}
