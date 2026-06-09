using UnityEngine;

public class Speaker : MonoBehaviour, IInteractable
{
    public AudioSource music;

    private bool isBroken = false;

    public bool IsBroken => isBroken;
    public bool CanInteract => !isBroken;

    public void Interact(InventorySystem inventorySystem)
    {
        if (isBroken)
            return;

        BreakSpeaker();
    }

    private void BreakSpeaker()
{
    isBroken = true;

    SpeakerManager.Instance.UpdateVolume();

    SpeakerManager.Instance.CheckAllSpeakersBroken();
}

    public void Reactivate()
    {
        isBroken = false;

        SpeakerManager.Instance.UpdateVolume();
    }
}