using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private RectTransform canvasRect;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void Update()
    {
        if (panel.activeSelf)
        {
            panel.transform.position = Input.mousePosition + new Vector3(15, -15, 0);
        }
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

        nameText.text = item.itemName;
        descriptionText.text = item.description;

        string stats = "";
        if (item.attackBonus > 0) stats += $"Atk +{item.attackBonus}\n";
        if (item.defenseBonus > 0) stats += $"Def +{item.defenseBonus}\n";
        if (item.healAmount > 0) stats += $"Heals {item.healAmount} HP\n";
        statsText.text = stats;

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}