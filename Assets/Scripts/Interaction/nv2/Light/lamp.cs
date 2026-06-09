using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    public Light lightSource;

    private bool isOn = false;

    public bool IsOn => isOn;

    private void Start()
    {
        isOn = false;
        if (lightSource != null)
            lightSource.enabled = false;
    }

    public void Interact(InventorySystem inventorySystem)
    {
        if (!isOn)
        {
            TurnOn();
        }
    }

    private void TurnOn()
{
    isOn = true;

    lightSource.enabled = true;

    LightController.Instance.CheckAllLightsOn();
}

    public void ForceTurnOff()
    {
        isOn = false;
        lightSource.enabled = false;
    }
}