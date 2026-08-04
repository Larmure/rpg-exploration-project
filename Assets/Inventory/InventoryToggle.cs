using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    void Start()
    {
        SetVisible(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isVisible = inventoryPanel.activeSelf;
            SetVisible(!isVisible);
        }
    }

    private void SetVisible(bool visible)
    {
        inventoryPanel.SetActive(visible);
        equipmentPanel.SetActive(visible);
    }
}