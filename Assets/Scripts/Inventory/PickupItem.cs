using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    public ItemDefinition item;
    public int quantity = 1;

    private void Reset()
    {
        Collider itemCollider = GetComponent<Collider>();
        itemCollider.isTrigger = true;
    }

    public bool TryPickup(InventorySystem inventory)
    {
        if (inventory == null || item == null)
        {
            return false;
        }

        bool added = inventory.AddItem(item, quantity);
        if (added)
        {
            Destroy(gameObject);
        }

        return added;
    }

    public void Interact(InventorySystem inventorySystem)
    {
        TryPickup(inventorySystem);
    }
}
