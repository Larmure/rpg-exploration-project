using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private enum ShopMode { Buy, Sell }

    [Header("UI References (même style que l'inventaire)")]
    public GameObject shopPanel;
    public Transform shopDisplay;

    private Sprite blankItem;
    private ShopSlotUI[] slotUIs;

    private ShopMode currentMode;
    private Merchant currentMerchant;

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

    // ---------------- OUVERTURE ----------------

    [Header("UI à masquer pendant le shop")]
    public GameObject inventoryUI; // à assigner: l'objet "Inventory" ou son parent

    public void OpenBuyMenu(Merchant merchant)
    {
        currentMode = ShopMode.Buy;
        currentMerchant = merchant;
        shopPanel.SetActive(true);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        LoadBuyMenu();
    }

    public void OpenSellMenu()
    {
        currentMode = ShopMode.Sell;
        currentMerchant = null;
        shopPanel.SetActive(true);
        if (inventoryUI != null) inventoryUI.SetActive(false);
        LoadSellMenu();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        if (inventoryUI != null) inventoryUI.SetActive(true);
        currentMerchant = null;
    }

    // ---------------- AFFICHAGE DES SLOTS ----------------

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

    private void LoadSellMenu()
    {
        Item[] inventory = InventoryManager.instance.inventories;

        for (int i = 0; i < shopDisplay.childCount; i++)
        {
            var icon = shopDisplay.GetChild(i).Find("Icon").GetComponent<Image>();
            var amountText = shopDisplay.GetChild(i).Find("Amount").GetComponent<TextMeshProUGUI>();

            Item item = i < inventory.Length ? inventory[i] : null;

            if (item == null)
            {
                icon.sprite = blankItem;
                amountText.text = "";
                continue;
            }

            icon.sprite = item.icon;
            amountText.text = "" + item.amount;
        }
    }

    // ---------------- SURVOL (PRIX) ----------------

    public void ShowPriceForSlot(int index)
    {
        if (slotUIs == null || index >= slotUIs.Length) return;

        Item item = GetItemForSlot(index);
        if (item == null) return;

        int price = GetPriceForSlot(index);
        Color color = currentMode == ShopMode.Sell ? Color.red : Color.white;

        slotUIs[index].ShowPrice(price.ToString(), color);
    }

    public void HidePriceForSlot(int index)
    {
        if (slotUIs == null || index >= slotUIs.Length) return;
        slotUIs[index].HidePrice();
    }

    private Item GetItemForSlot(int index)
    {
        if (currentMode == ShopMode.Buy)
        {
            if (currentMerchant == null || index >= currentMerchant.itemsForSale.Length) return null;
            return currentMerchant.itemsForSale[index];
        }
        else
        {
            if (index >= InventoryManager.instance.inventories.Length) return null;
            return InventoryManager.instance.GetItem(index);
        }
    }

    private int GetPriceForSlot(int index)
    {
        Item item = GetItemForSlot(index);
        if (item == null) return 0;

        return currentMode == ShopMode.Buy ? item.price : GetSellPrice(item);
    }

    private int GetSellPrice(Item item)
    {
        // Prix de revente = moitié du prix d'achat, arrondi, minimum 1.
        return Mathf.Max(1, Mathf.RoundToInt(item.price * 0.5f));
    }

    // ---------------- CLIC (ACHAT / VENTE) ----------------

    public void OnSlotClicked(int index)
    {
        if (currentMode == ShopMode.Buy)
        {
            BuyItem(index);
        }
        else
        {
            SellItem(index);
        }
    }

    private void BuyItem(int index)
    {
        if (currentMerchant == null) return;
        if (index >= currentMerchant.itemsForSale.Length) return;

        Item template = currentMerchant.itemsForSale[index];
        if (template == null) return;

        if (Player.instance.gold < template.price)
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

        Player.instance.gold -= template.price;
        LoadBuyMenu();
    }

    private void SellItem(int index)
    {
        Item item = InventoryManager.instance.GetItem(index);
        if (item == null) return;

        if (item.ItemType == ItemType.coin)
        {
            Debug.Log("Impossible de vendre des pièces.");
            return;
        }

        int sellPrice = GetSellPrice(item);
        Player.instance.gold += sellPrice;

        InventoryManager.instance.SetItem(index, null);
        InventoryManager.instance.RefreshUI();
        Destroy(item.gameObject);

        LoadSellMenu();
    }
}
