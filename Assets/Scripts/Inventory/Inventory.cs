using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
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

    public void LoadInventory()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            int index = i;

            if (inventories[i] == null)
            {
                inventoryDisplay.transform.GetChild(index).transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "";
                inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Image>().sprite = blankItem;
                inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.RemoveAllListeners();
                inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.AddListener(
                    delegate {
                        Debug.Log("Empty Slot");
                        SetSlot(index);
                    });

                continue;
            }

            inventoryDisplay.transform.GetChild(index).transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "" + inventories[i].amount;
            inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Image>().sprite = inventories[i].icon;

            inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.RemoveAllListeners();
            inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.AddListener(
                delegate {
                    inventories[index].UseItem();
                    SetSlot(index);
                });
            inventoryDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().interactable = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void SetSlot(int index)
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