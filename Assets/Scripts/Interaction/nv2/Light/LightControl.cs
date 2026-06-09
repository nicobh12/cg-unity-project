using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LightController : MonoBehaviour

{
    public List<LightSwitch> lights = new List<LightSwitch>();
    public float interval = 30f;
    public GhostMessageUI ghostUI;
    public static LightController Instance;

    private bool lightsCompleted = false;

    public bool LightsCompleted => lightsCompleted;

    private void Start()
    {
        StartCoroutine(ControlLoop());
    }

    IEnumerator ControlLoop()
{
    while (true)
    {
        yield return new WaitForSeconds(interval);

        if (AllLightsOn())
        {
            yield break;
        }

        TurnOffRandomLight();
    }
}

    bool AllLightsOn()
{
    foreach (var l in lights)
    {
        if (l != null && !l.IsOn)
            return false;
    }

    return true;
}

private void Awake()
{
    Instance = this;
}

    void TurnOffRandomLight()
    {
        if (lights.Count == 0) return;

        List<LightSwitch> onLights = new List<LightSwitch>();

        foreach (var l in lights)
        {
            if (l != null && l.IsOn)
                onLights.Add(l);
        }

        if (onLights.Count == 0) return;

        int index = Random.Range(0, onLights.Count);
        onLights[index].ForceTurnOff();

        if (ghostUI != null)
        {
            ghostUI.ShowMessage("¡Oh no! Un fantasma apagó una luz");
        }
    }

    public void CheckAllLightsOn()
{
    if (lightsCompleted)
        return;

    foreach (var l in lights)
    {
        if (l != null && !l.IsOn)
            return;
    }

    lightsCompleted = true;

    if (ghostUI != null)
    {
        ghostUI.ShowMessage(
            "¡Felicidades! Has encendido todas las luces",
            4f
        );
    }
}
}