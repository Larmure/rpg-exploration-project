using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentManager : MonoBehaviour, IItemContainer
{
    public static EquipmentManager instance;
    public Item[] equipments = new Item[3];
    public Transform equipmentDisplay;
    private bool isEquipmentOpen = false;
    private Sprite blankItem;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        blankItem = equipmentDisplay.transform.GetChild(0).transform.Find("Icon").GetComponent<Image>().sprite;
        equipmentDisplay.gameObject.SetActive(false);
    }

    // --- IItemContainer ---

    public Item GetItem(int index)
    {
        if (index < 0 || index >= equipments.Length) return null;
        return equipments[index];
    }

    public void SetItem(int index, Item item)
    {
        if (index < 0 || index >= equipments.Length) return;
        equipments[index] = item;
    }

    public void RefreshUI()
    {
        LoadEquipment();
    }

    // ----------------------

    public void LoadEquipment()
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            var icon = equipmentDisplay.transform.GetChild(i).transform.Find("Icon").GetComponent<Image>();
            var amountText = equipmentDisplay.transform.GetChild(i).transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            if (equipments[i] == null)
            {
                amountText.text = "";
                icon.sprite = blankItem;
                continue;
            }

            amountText.text = "" + equipments[i].amount;
            icon.sprite = equipments[i].icon;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleEquipment();
        }
    }

    private void ToggleEquipment()
    {
        isEquipmentOpen = !isEquipmentOpen;
        equipmentDisplay.gameObject.SetActive(isEquipmentOpen);

        if (isEquipmentOpen)
        {
            LoadEquipment();
        }
    }
}
