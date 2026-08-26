using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarManager : MonoBehaviour, IItemContainer
{
    public static HotbarManager instance;
    public Item[] inventories = new Item[6];
    public Transform hotbarDisplay;
    private Sprite blankItem;
    public int currentHotbarIndex = 0;
    public Item currentItem;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        blankItem = hotbarDisplay.transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite;
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

        if (index == currentHotbarIndex)
        {
            currentItem = item;
        }
    }

    public void RefreshUI()
    {
        LoadHotbar();
    }

    // ----------------------

    public void LoadHotbar()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            var icon = hotbarDisplay.transform.GetChild(i).transform.Find("Icon").GetComponent<Image>();
            var amountText = hotbarDisplay.transform.GetChild(i).transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            if (inventories[i] == null)
            {
                amountText.text = "";
                icon.sprite = blankItem;
                continue;
            }

            amountText.text = "" + inventories[i].amount;
            icon.sprite = inventories[i].icon;
        }
        currentItem = inventories[currentHotbarIndex];
    }

    public void SetSlot(int index)
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
