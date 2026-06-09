using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const string DDRTutorialSeenKey = "DDRTutorialSeen";

    [Header("Level 1 - Puzzle")]
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

    [Header("Persistent Stats")]
    [SerializeField] private float savedPlayerMaxHealth = 100f;
    [SerializeField] private float savedPlayerHealth = 100f;
    [SerializeField] private float savedBossMaxHealth = 100f;
    [SerializeField] private float savedBossHealth = 100f;
    [SerializeField] private float partyOMeter = 0f;
    [SerializeField] private float maxPartyOMeter = 100f;

    [Header("DDR")]
    [SerializeField] private string ddrSceneName = "ddr";
    [SerializeField] private string ddrReturnSceneName = "nv3";
    [SerializeField] private float ddrFailMissThreshold = 0.30f;
    [SerializeField] private float ddrFailHealthPenalty = 20f;
    [SerializeField] private float ddrFailPartyOMeterGain = 15f;

    [Header("DDR Progress")]
    [SerializeField] private int ddrEntries = 0;
    [SerializeField] private int maxDDREntriesBeforeDeath = 3;

    public int DDREntries => ddrEntries;
    public bool IsFirstDDREntry => ddrEntries == 1;
    public bool HasSeenDDRTutorial => PlayerPrefs.GetInt(DDRTutorialSeenKey, 0) == 1;
    public bool ShouldShowDDRTutorialOnCurrentEntry => IsFirstDDREntry && !HasSeenDDRTutorial;

    [Header("Player Discovery")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string bossTag = "Boss";

    [SerializeField] private List<InventorySystem.InventoryEntry> inventorySnapshot = new List<InventorySystem.InventoryEntry>();

    private Health currentPlayerHealth;
    private InventorySystem currentPlayerInventory;
    private Health currentBossHealth;
    private bool isTransitioningToDDR;
    private bool isDDRActive;
    private bool hasInitializedFromScene;
    private string returnSceneName;

    public float PlayerHealth => savedPlayerHealth;
    public float PlayerMaxHealth => savedPlayerMaxHealth;
    public float BossHealth => savedBossHealth;
    public float BossMaxHealth => savedBossMaxHealth;
    public float PartyOMeter => partyOMeter;
    public float MaxPartyOMeter => maxPartyOMeter;
    public bool IsDDRActive => isDDRActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        BindAndApplyPlayerState();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnEnable()
{
    if (timelineDirector != null)
    {
        timelineDirector.stopped += OnTimelineFinished;
    }
}

    private void LateUpdate()
    {
        if (!isTransitioningToDDR && SceneManager.GetActiveScene().name != ddrSceneName)
        {
            CapturePlayerState();
        }
    }

    public void MarkDDRTutorialAsSeen()
    {
        if (HasSeenDDRTutorial)
        {
            return;
        }

        PlayerPrefs.SetInt(DDRTutorialSeenKey, 1);
        PlayerPrefs.Save();
    }

    public void RegisterPlayer(Health playerHealth, InventorySystem playerInventory)
    {
        currentPlayerHealth = playerHealth;
        currentPlayerInventory = playerInventory;
        ApplyStateToCurrentPlayer();
    }

    public void CapturePlayerState()
    {
        if (currentPlayerHealth != null)
        {
            savedPlayerMaxHealth = currentPlayerHealth.MaxHealth;
            savedPlayerHealth = currentPlayerHealth.CurrentHealth;
        }

        if (currentPlayerInventory != null)
        {
            inventorySnapshot = currentPlayerInventory.CreateSnapshot();
        }

        if (currentBossHealth != null)
        {
            savedBossMaxHealth = currentBossHealth.MaxHealth;
            savedBossHealth = currentBossHealth.CurrentHealth;
        }
    }

    public void SetPartyOMeter(float value)
    {
        partyOMeter = Mathf.Clamp(value, 0f, maxPartyOMeter);
    }

    public void AddPartyOMeter(float amount)
    {
        SetPartyOMeter(partyOMeter + amount);
    }

    public void ReducePartyOMeter(float amount)
    {
        AddPartyOMeter(-amount);
    }

    public void ResetRuntimeStateForRetry()
    {
        savedPlayerHealth = savedPlayerMaxHealth;
        savedBossHealth = savedBossMaxHealth;
        partyOMeter = maxPartyOMeter;
        ddrEntries = 0;
        isDDRActive = false;
        isTransitioningToDDR = false;
    }

    public void StartDDRChallengeFromSpecialWave()
    {
        Debug.Log("Starting DDR Challenge from Special Wave");
        
        if (isTransitioningToDDR)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == ddrSceneName)
        {
            return;
        }

        ddrEntries++;
        if (ddrEntries > maxDDREntriesBeforeDeath)
        {
            if (currentPlayerHealth != null)
            {
                //Set health to 0 to trigger death
                currentPlayerHealth.SetCurrentHealth(0f);
            }
            return;
        }

        CapturePlayerState();
        returnSceneName = SceneManager.GetActiveScene().name;
        isTransitioningToDDR = true;
        isDDRActive = true;
        StartCoroutine(LoadSceneRoutine(ddrSceneName));
    }

    public void CompleteDDRChallenge(float missRatio)
    {
        if (missRatio > ddrFailMissThreshold)
        {
            savedPlayerHealth = Mathf.Max(0f, savedPlayerHealth - ddrFailHealthPenalty);
            AddPartyOMeter(ddrFailPartyOMeterGain);
        }

        isTransitioningToDDR = true;
        string sceneToLoad = string.IsNullOrWhiteSpace(ddrReturnSceneName)
            ? (string.IsNullOrWhiteSpace(returnSceneName) ? "Main" : returnSceneName)
            : ddrReturnSceneName;

        StartCoroutine(LoadSceneRoutine(sceneToLoad));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        if (sceneName != ddrSceneName)
        {
            isDDRActive = false;
        }

        isTransitioningToDDR = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindAndApplyPlayerState();
    }

    private void BindAndApplyPlayerState()
    {
        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.onDied.RemoveListener(HandlePlayerDeath);
        }

        if (currentBossHealth != null)
        {
            currentBossHealth.onDied.RemoveListener(HandleBossDeath);
        }

        currentPlayerHealth = null;
        currentPlayerInventory = null;
        currentBossHealth = null;

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            currentPlayerHealth = playerObject.GetComponent<Health>();
            currentPlayerInventory = playerObject.GetComponent<InventorySystem>();
        }

        if (currentPlayerHealth == null)
        {
            currentPlayerHealth = FindAnyObjectByType<Health>();
        }

        if (currentPlayerInventory == null)
        {
            currentPlayerInventory = FindAnyObjectByType<InventorySystem>();
        }

        GameObject bossObject = GameObject.FindGameObjectWithTag(bossTag);
        if (bossObject != null)
        {
            currentBossHealth = bossObject.GetComponent<Health>();
        }

        if (!hasInitializedFromScene && (currentPlayerHealth != null || currentPlayerInventory != null || currentBossHealth != null))
        {
            CapturePlayerState();
            hasInitializedFromScene = true;
        }

        ApplyStateToCurrentPlayer();

        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.onDied.AddListener(HandlePlayerDeath);
        }

        if (currentBossHealth != null)
        {
            currentBossHealth.onDied.AddListener(HandleBossDeath);
        }
    }

    private void ApplyStateToCurrentPlayer()
    {
        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.SetMaxHealth(savedPlayerMaxHealth, false);
            currentPlayerHealth.SetCurrentHealth(savedPlayerHealth);
        }

        if (currentPlayerInventory != null)
        {
            currentPlayerInventory.LoadSnapshot(inventorySnapshot);
        }

        if (currentBossHealth != null)
        {
            currentBossHealth.SetMaxHealth(savedBossMaxHealth, false);
            currentBossHealth.SetCurrentHealth(savedBossHealth);
        }
    }

    private void HandlePlayerDeath()
    {
        if (PermanentHUDManager.Instance != null)
        {
            PermanentHUDManager.Instance.ShowGameOver();
        }
    }

    private void HandleBossDeath()
    {
        SetPartyOMeter(0f);

        if (PartyometerController.Instance != null)
        {
            PartyometerController.Instance.ForceToZero();
        }
    }

        private void OnTimelineFinished(PlayableDirector director)
    {
        if (seguirJugador != null)
        {
            seguirJugador.SetActive(true);
        }

        if (player != null)
        {
            player.SetActive(true);
        }

        Debug.Log("Timeline terminado");
    }

    public void EncontrarPista1()
    {
        p1 = "5";
        find1 = true;
        ActivarLlave();
    }

    public void EncontrarPista2()
    {
        p2 = "1";
        find2 = true;
        ActivarLlave();
    }

    public void EncontrarPista3()
    {
        p3 = "9";
        find3 = true;
        ActivarLlave();
    }

    private void ActivarLlave()
    {
        if (find1 && find2 && find3)
        {
            keyFound = true;

            if (key != null)
            {
                key.SetActive(true);
            }
        }
    }
}
