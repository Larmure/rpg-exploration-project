// Interface implémentée par InventoryManager, HotbarManager et EquipmentManager
// pour permettre au système de drag & drop de manipuler n'importe quel conteneur
// de façon générique, sans dupliquer de logique.
public interface IItemContainer
{
    Item GetItem(int index);
    void SetItem(int index, Item item);

    // Recharge visuellement les slots (icônes, texte, boutons...)
    void RefreshUI();
}
