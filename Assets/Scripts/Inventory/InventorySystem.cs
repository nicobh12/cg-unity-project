using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable]
    public class InventoryEntry
    {
        public ItemDefinition item;
        public int quantity;
    }

    [SerializeField] private List<InventoryEntry> items = new List<InventoryEntry>();

    public IReadOnlyList<InventoryEntry> Items => items;

    public bool AddItem(ItemDefinition item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        if (item.stackable)
        {
            InventoryEntry existingEntry = items.Find(entry => entry.item == item);
            if (existingEntry != null)
            {
                existingEntry.quantity = Mathf.Min(existingEntry.quantity + quantity, item.maxStackSize);
                return true;
            }
        }

        items.Add(new InventoryEntry
        {
            item = item,
            quantity = item.stackable ? Mathf.Min(quantity, item.maxStackSize) : 1
        });

        return true;
    }

    public bool HasItem(ItemDefinition item)
    {
        return items.Exists(entry => entry.item == item);
    }

    public int GetQuantity(ItemDefinition item)
    {
        InventoryEntry entry = items.Find(candidate => candidate.item == item);
        return entry != null ? entry.quantity : 0;
    }
}
