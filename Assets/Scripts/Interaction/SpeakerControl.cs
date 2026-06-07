using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerManager : MonoBehaviour
{
    public static SpeakerManager Instance;

    public AudioSource backgroundMusic;
    public List<Speaker> speakers;

    [SerializeField] private float reactivateTime = 20f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ReactivationLoop());
    }

    public void UpdateVolume()
    {
        int active = 0;

        foreach (Speaker s in speakers)
        {
            if (!s.IsBroken)
                active++;
        }

        if (speakers.Count > 0)
            backgroundMusic.volume = (float)active / speakers.Count;
    }

    private IEnumerator ReactivationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(reactivateTime);

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

                UpdateVolume();
            }
        }
    }
}