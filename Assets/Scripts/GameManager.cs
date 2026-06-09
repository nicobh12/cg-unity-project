using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UIDocument uiDocument;
    public PlayableDirector timelineDirector;

    public bool vioPuerta = false;
    public bool vioMesita = false;

    public string p1 = "_____";
    public string p2 = "_____";
    public string p3 = "_____";

    public GameObject seguirJugador;
    public GameObject player;
    public GameObject key;
    
    private bool find1 = false;
    private bool find2 = false;
    private bool find3 = false;
    
    public bool keyFound = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineFinished;
        }
    }

    private void OnDisable()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        // Lo que quieras hacer cuando termine el Timeline

        seguirJugador.SetActive(true);
        player.SetActive(true);

        Debug.Log("Timeline terminado");
    }

    public void EncontrarPista1()
    {
        
        p1 = "5";
        find1 = true;
        activarLlave();
    }

    public void EncontrarPista2()
    {
        p2 = "1";
        find2 = true;
        activarLlave();
    }

    public void EncontrarPista3()
    {
        p3 = "9";
        find3 = true;
        activarLlave();
    }

    void activarLlave()
    {
        if (find1 && find2 && find3)
        {
            keyFound = true;
            key.SetActive(true);
        }
    }
}