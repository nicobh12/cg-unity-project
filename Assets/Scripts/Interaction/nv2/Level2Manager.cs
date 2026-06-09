using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public LightController lightController;
    public SpeakerManager speakerManager;

    private bool levelCompleted = false;

    private void Update()
    {
        if (levelCompleted)
            return;

        if (AllLightsOn() && AllSpeakersOff())
        {
            levelCompleted = true;
            StartCoroutine(CompleteLevel());
        }
    }

    IEnumerator CompleteLevel()
    {
        Debug.Log("¡Nivel completado!");

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("nv3");
    }

    bool AllLightsOn()
    {
        foreach (var light in lightController.lights)
        {
            if (light != null && !light.IsOn)
                return false;
        }

        return true;
    }

    bool AllSpeakersOff()
    {
        foreach (var speaker in speakerManager.speakers)
        {
            if (speaker != null && !speaker.IsBroken)
                return false;
        }

        return true;
    }
}