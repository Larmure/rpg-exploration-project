using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI References (même style que l'inventaire)")]
    public GameObject shopPanel;
    public Transform shopDisplay;

    private Sprite blankItem;
    private ShopSlotUI[] slotUIs;
    private Merchant currentMerchant;

    public bool IsShopOpen => shopPanel != null && shopPanel.activeSelf;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        slotUIs = shopDisplay.GetComponentsInChildren<ShopSlotUI>(true);
        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].Setup(i, this);
        }

        blankItem = shopDisplay.GetChild(0).Find("Icon").GetComponent<Image>().sprite;

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void Update()
    {
        if (shopPanel != null && shopPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    // ---------------- OUVERTURE / FERMETURE ----------------

    [Header("UI à masquer pendant le shop")]
    public GameObject inventoryUI;
    public GameObject equipmentUI;

    public void OpenShop(Merchant merchant)
    {
        currentMerchant = merchant;
        shopPanel.SetActive(true);
        if (inventoryUI != null) inventoryUI.SetActive(true);
        if (equipmentUI != null) equipmentUI.SetActive(true);
        LoadBuyMenu();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        if (equipmentUI != null) equipmentUI.SetActive(false);
        currentMerchant = null;
    }

    // ---------------- AFFICHAGE DU SHOP (ACHAT) ----------------

    private void LoadBuyMenu()
    {
        Item[] stock = currentMerchant != null ? currentMerchant.itemsForSale : new Item[0];

        for (int i = 0; i < shopDisplay.childCount; i++)
        {
            var icon = shopDisplay.GetChild(i).Find("Icon").GetComponent<Image>();
            var amountText = shopDisplay.GetChild(i).Find("Amount").GetComponent<TextMeshProUGUI>();

            if (i >= stock.Length || stock[i] == null)
            {
                icon.sprite = blankItem;
                amountText.text = "";
                continue;
            }

            icon.sprite = stock[i].icon;
            amountText.text = "";
        }
    }

    // ---------------- SURVOL (PRIX D'ACHAT) ----------------

    public void ShowPriceForSlot(int index)
    {
        if (slotUIs == null || index >= slotUIs.Length) return;

        Item item = GetItemForSlot(index);
        if (item == null) return;

        slotUIs[index].ShowPrice(item.price.ToString(), Color.white);
    }

    public void HidePriceForSlot(int index)
    {
        if (slotUIs == null || index >= slotUIs.Length) return;
        slotUIs[index].HidePrice();
    }

    private Item GetItemForSlot(int index)
    {
        if (currentMerchant == null || index >= currentMerchant.itemsForSale.Length) return null;
        return currentMerchant.itemsForSale[index];
    }

    // ---------------- CLIC SUR LE SHOP = ACHAT ----------------

    public void OnSlotClicked(int index)
    {
        BuyItem(index);
    }

    private void BuyItem(int index)
    {
        if (currentMerchant == null) return;
        if (index >= currentMerchant.itemsForSale.Length) return;

        Item template = currentMerchant.itemsForSale[index];
        if (template == null) return;

        if (GetGold() < template.price)
        {
            Debug.Log("Pas assez d'or !");
            return;
        }

        Item newItem = Instantiate(template);
        newItem.gameObject.SetActive(true);
        newItem.isUsed = true;

        var sr = newItem.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        foreach (var col in newItem.GetComponents<Collider2D>())
            col.enabled = false;

        bool added = newItem.AddItemToHotbar();
        if (!added) added = newItem.AddItemToInventory();

        if (!added)
        {
            Debug.Log("Hotbar et inventaire pleins, achat annulé.");
            Destroy(newItem.gameObject);
            return;
        }

        SetGold(GetGold() - template.price);
        // Retire l'item du stock du marchand (achat unique par slot)
        currentMerchant.itemsForSale[index] = null;
        LoadBuyMenu();

    }

    // ---------------- CLIC SUR HOTBAR / INVENTORY / EQUIPMENT = VENTE ----------------

    public void SellItem(IItemContainer container, int slotIndex, ContainerType containerType)
    {
        if (container == null) return;

        Item item = container.GetItem(slotIndex);
        if (item == null) return;

        if (item.ItemType == ItemType.coin)
        {
            Debug.Log("Impossible de vendre des pièces.");
            return;
        }

        if (containerType == ContainerType.Equipment)
            (item as EquipmentItem)?.NotifyUnequipped();

        int sellPrice = GetSellPrice(item);
        SetGold(GetGold() + sellPrice);

        container.SetItem(slotIndex, null);
        container.RefreshUI();
        Destroy(item.gameObject);
    }

    private int GetSellPrice(Item item)
    {
        // Prix de revente = moitié du prix d'achat, arrondi, minimum 1.
        return Mathf.Max(1, Mathf.RoundToInt(item.price * 0.5f));
    }

    private Component FindGoldOwner(out FieldInfo field)
    {
        foreach (var component in FindObjectsOfType<MonoBehaviour>())
        {
            field = component.GetType().GetField("gold",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null && field.FieldType == typeof(int)) return component;
        }

        field = null;
        return null;
    }

    private int GetGold()
    {
        FieldInfo field;
        Component owner = FindGoldOwner(out field);
        return owner == null ? 0 : (int)field.GetValue(owner);
    }

    private void SetGold(int value)
    {
        FieldInfo field;
        Component owner = FindGoldOwner(out field);
        if (owner != null) field.SetValue(owner, value);
    }
}