using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public Item[] inventories = new Item[6];
    public Transform inventoryDisplay;
    private Sprite blankItem;
    public int currentInventoryIndex = 0;
    public Item currentItem;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        blankItem = inventoryDisplay.transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite;
    }

    public void LoadInventory()
    {
        for (int i = 0; i < inventories.Length; i++)
        {

            int index = i;

            if(inventories[i] == null)
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

    private void SetSlot(int index)
    {
        foreach (Transform slot in inventoryDisplay)
        {
            slot.transform.Find("Outline").gameObject.SetActive(false);
        }
        inventoryDisplay.transform.GetChild(index).transform.Find("Outline").gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentInventoryIndex++;
            if (currentInventoryIndex % inventories.Length == 0)
            {
                currentInventoryIndex = 0;
            }
            SetSlot(currentInventoryIndex);
            currentItem = inventories[currentInventoryIndex];
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentInventoryIndex = 0;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentInventoryIndex = 1;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentInventoryIndex = 2;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentInventoryIndex = 3;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentInventoryIndex = 4;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentInventoryIndex = 5;
            SetSlot(currentInventoryIndex);
            if(inventories[currentInventoryIndex] != null)
                inventories[currentInventoryIndex].UseItem();
        }
    }
}
