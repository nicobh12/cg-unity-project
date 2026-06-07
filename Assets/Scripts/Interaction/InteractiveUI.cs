using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public InteractionProximityDetector detector;
    public GameObject interactText;

    void Update()
    {
        bool show = false;

        var field = typeof(InteractionProximityDetector)
            .GetField("nearbyInteractables",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        var list = field.GetValue(detector) as HashSet<IInteractable>;

        foreach (var obj in list)
        {
            if (obj is Speaker speaker)
            {
                if (!speaker.IsBroken)
                {
                    show = true;
                    break;
                }
            }
            else
            {
                show = true;
                break;
            }
        }

        interactText.SetActive(show);
    }
}