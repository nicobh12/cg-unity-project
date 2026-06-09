using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractionProximityDetector : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private readonly HashSet<IInteractable> nearbyInteractables = new HashSet<IInteractable>();
    private InventorySystem inventorySystem;

    public bool CanInteract => nearbyInteractables.Count > 0;

    private void Awake()
    {
        inventorySystem = GetComponentInParent<InventorySystem>();

        Collider detectorCollider = GetComponent<Collider>();
        detectorCollider.isTrigger = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonoBehaviour[] behaviours = other.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
            {
                nearbyInteractables.Add(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MonoBehaviour[] behaviours = other.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
            {
                nearbyInteractables.Remove(interactable);
            }
        }
    }

    public void TryInteract()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GetComponentInParent<InventorySystem>();
        }

        List<IInteractable> invalidInteractables = new List<IInteractable>();

        foreach (IInteractable interactable in nearbyInteractables)
        {
            MonoBehaviour behaviour = interactable as MonoBehaviour;

            if (behaviour == null)
            {
                invalidInteractables.Add(interactable);
                continue;
            }

            interactable.Interact(inventorySystem);
        }

        foreach (IInteractable invalid in invalidInteractables)
        {
            nearbyInteractables.Remove(invalid);
        }
    }
}
