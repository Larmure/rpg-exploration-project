using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentManager : MonoBehaviour
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

    public void LoadEquipment()
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            int index = i;

            var icon = equipmentDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Image>();
            var button = equipmentDisplay.transform.GetChild(index).transform.Find("Icon").GetComponent<Button>();
            var amountText = equipmentDisplay.transform.GetChild(index).transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            button.onClick.RemoveAllListeners();

            if (equipments[i] == null)
            {
                amountText.text = "";
                icon.sprite = blankItem;
                button.interactable = false;

                continue;
            }

            amountText.text = "" + equipments[i].amount;
            icon.sprite = equipments[i].icon;

            button.interactable = true;
            button.onClick.AddListener(delegate
            {
                (equipments[index] as EquipmentItem)?.UnequipItem();
            });
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