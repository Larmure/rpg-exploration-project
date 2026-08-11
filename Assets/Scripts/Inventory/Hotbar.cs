using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;
    public Item[] inventories = new Item[6];
    public Transform hotbarDisplay;
    private Sprite blankItem;
    public int currentHotbarIndex = 0;
    public Item currentItem;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        blankItem = hotbarDisplay.transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite;
    }

    public void LoadHotbar()
    {
        for (int i = 0; i < inventories.Length; i++)
        {

            int index = i;

            if(inventories[i] == null)
            {
                hotbarDisplay.transform.GetChild(index).transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "";
                hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Image>().sprite = blankItem;
                hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.RemoveAllListeners();
                hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.AddListener(
                    delegate { 
                        Debug.Log("Empty Slot");
                        SetSlot(index);
                        });

                continue;   
            }

            hotbarDisplay.transform.GetChild(index).transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "" + inventories[i].amount;
            hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Image>().sprite = inventories[i].icon;

            hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.RemoveAllListeners();
            hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().onClick.AddListener(
                delegate { 
                        inventories[index].UseItem();
                        SetSlot(index);
                        });
            hotbarDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>().interactable = true;

        }
        currentItem = inventories[currentHotbarIndex];
    }

    private void SetSlot(int index)
    {
        foreach (Transform slot in hotbarDisplay)
        {
            slot.transform.Find("Outline").gameObject.SetActive(false);
        }
        hotbarDisplay.transform.GetChild(index).transform.Find("Outline").gameObject.SetActive(true);
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentHotbarIndex += scroll > 0 ? 1 : -1;
            if (currentHotbarIndex < 0) currentHotbarIndex = inventories.Length - 1;
            if (currentHotbarIndex >= inventories.Length) currentHotbarIndex = 0;
            SelectSlot(currentHotbarIndex);
        }

        for (int i = 0; i < inventories.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    private void SelectSlot(int index)
    {
        currentHotbarIndex = index;
        SetSlot(index);
        currentItem = inventories[index];
    }
}
