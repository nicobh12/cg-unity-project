using UnityEngine;

public class InteractableExample : MonoBehaviour, IInteractable
{
    public void Interact(InventorySystem inventorySystem)
    {
        Debug.Log($"Interacted with {name}");
    }
}
