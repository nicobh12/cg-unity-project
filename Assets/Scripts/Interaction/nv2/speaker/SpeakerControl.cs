using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeakerManager : MonoBehaviour
{
    public GhostMessageUI ghostUI;
    public static SpeakerManager Instance;
    private bool speakersCompleted = false;

    public bool SpeakersCompleted => speakersCompleted;
    public List<Speaker> speakers;

    [SerializeField] private float reactivateTime = 35f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ReactivationLoop());
    }

    public void CheckAllSpeakersBroken()
{
    if (speakersCompleted)
        return;

    foreach (Speaker s in speakers)
    {
        if (!s.IsBroken)
            return;
    }

    speakersCompleted = true;

    if (ghostUI != null)
    {
        ghostUI.ShowMessage(
            "¡Felicidades! Has apagado todos los parlantes",
            4f
        );
    }
}

    public void UpdateVolume()
    {
        int active = 0;

        foreach (Speaker s in speakers)
        {
            if (!s.IsBroken)
                active++;
        }

        if (speakers.Count > 0){
        
            float minVolume = 0.20f;

            MusicManager.Instance.SetVolume(
                Mathf.Lerp(minVolume, 1f, (float)active / speakers.Count)
            );
        }
    }

    bool AllSpeakersBroken()
{
    foreach (Speaker s in speakers)
    {
        if (!s.IsBroken)
            return false;
    }

    return true;
}

    private IEnumerator ReactivationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(reactivateTime);

            if (AllSpeakersBroken())
            {
                yield break;
            }

            List<Speaker> broken = new List<Speaker>();

            foreach (Speaker s in speakers)
            {
                if (s.IsBroken)
                    broken.Add(s);
            }

            if (broken.Count > 0)
            {
                int randomIndex = Random.Range(0, broken.Count);
                broken[randomIndex].Reactivate();

                if (ghostUI != null)
                {
                    ghostUI.ShowMessage("¡Oh no! Un fantasma encendió un parlante");
                }

                UpdateVolume();
            }
        }
    }
}