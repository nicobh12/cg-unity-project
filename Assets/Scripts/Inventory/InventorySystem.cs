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

    public ItemDefinition GetItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        InventoryEntry entry = items.Find(candidate => candidate.item != null && candidate.item.itemId == itemId);
        return entry != null ? entry.item : null;
    }

    public bool ConsumeItem(string itemId, int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0)
        {
            return false;
        }

        InventoryEntry entry = items.Find(candidate => candidate.item != null && candidate.item.itemId == itemId);
        if (entry == null || entry.quantity < quantity)
        {
            return false;
        }

        entry.quantity -= quantity;
        if (entry.quantity <= 0)
        {
            items.Remove(entry);
        }

        return true;
    }

    public int GetQuantity(ItemDefinition item)
    {
        InventoryEntry entry = items.Find(candidate => candidate.item == item);
        return entry != null ? entry.quantity : 0;
    }

    public List<InventoryEntry> CreateSnapshot()
    {
        List<InventoryEntry> snapshot = new List<InventoryEntry>();

        foreach (InventoryEntry entry in items)
        {
            if (entry == null || entry.item == null || entry.quantity <= 0)
            {
                continue;
            }

            snapshot.Add(new InventoryEntry
            {
                item = entry.item,
                quantity = entry.quantity
            });
        }

        return snapshot;
    }

    public void LoadSnapshot(List<InventoryEntry> snapshot)
    {
        items.Clear();
        if (snapshot == null)
        {
            return;
        }

        foreach (InventoryEntry entry in snapshot)
        {
            if (entry == null || entry.item == null || entry.quantity <= 0)
            {
                continue;
            }

            AddItem(entry.item, entry.quantity);
        }
    }
}
