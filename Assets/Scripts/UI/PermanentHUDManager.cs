using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PermanentHUDManager : MonoBehaviour
{
    public static PermanentHUDManager Instance { get; private set; }

    [Header("Roots")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private GameObject stopMenuRoot;
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private GameObject healthRoot;
    [SerializeField] private GameObject inventoryRoot;

    [Header("Bars")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider partyometerSlider;

    [Header("Retry")]
    [SerializeField] private string retrySceneName = "nv3";

    private Health currentPlayerHealth;
    private InventorySystem currentPlayerInventory;
    private bool stopMenuOpen;
    private bool gameOverOpen;
    private bool cutsceneLocked;

    public bool IsGameplayLocked => stopMenuOpen || gameOverOpen || cutsceneLocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        RefreshSceneBindings();
        HideStopMenu();
        HideGameOver();
        SetCutsceneLock(false);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindCurrentPlayer();
    }

    public void ToggleStopMenu()
    {
        if (gameOverOpen || cutsceneLocked)
        {
            return;
        }

        if (stopMenuOpen)
        {
            HideStopMenu();
            return;
        }

        ShowStopMenu();
    }

    public void ShowStopMenu()
    {
        if (gameOverOpen || cutsceneLocked)
        {
            return;
        }

        stopMenuOpen = true;
        SetMenuVisible(stopMenuRoot, true);
        ApplyPauseState();
    }

    public void HideStopMenu()
    {
        stopMenuOpen = false;
        SetMenuVisible(stopMenuRoot, false);
        ApplyPauseState();
    }

    public void ShowGameOver()
    {
        gameOverOpen = true;
        stopMenuOpen = false;
        SetMenuVisible(stopMenuRoot, false);
        SetMenuVisible(gameOverRoot, true);
        ApplyPauseState();
    }

    public void HideGameOver()
    {
        gameOverOpen = false;
        SetMenuVisible(gameOverRoot, false);
        ApplyPauseState();
    }

    public void SetCutsceneLock(bool locked)
    {
        cutsceneLocked = locked;

        if (locked)
        {
            HideStopMenu();
        }
        else if (!gameOverOpen)
        {
            ApplyPauseState();
        }
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRuntimeStateForRetry();
        }

        SceneManager.LoadScene(string.IsNullOrWhiteSpace(retrySceneName) ? SceneManager.GetActiveScene().name : retrySceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneBindings();
        HideStopMenu();
        HideGameOver();
    }

    private void RefreshSceneBindings()
    {
        UnbindCurrentPlayer();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            UpdatePartyometerUI();
            return;
        }

        currentPlayerHealth = playerObject.GetComponent<Health>();
        currentPlayerInventory = playerObject.GetComponent<InventorySystem>();

        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.onHealthChanged.AddListener(HandleHealthChanged);
            currentPlayerHealth.onDied.AddListener(HandlePlayerDied);
            HandleHealthChanged(currentPlayerHealth.CurrentHealth, currentPlayerHealth.MaxHealth);
        }

        if (healthRoot != null)
        {
            healthRoot.SetActive(currentPlayerHealth != null);
        }

        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(currentPlayerInventory != null);
        }

        UpdatePartyometerUI();
    }

    private void UnbindCurrentPlayer()
    {
        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.onHealthChanged.RemoveListener(HandleHealthChanged);
            currentPlayerHealth.onDied.RemoveListener(HandlePlayerDied);
        }

        currentPlayerHealth = null;
        currentPlayerInventory = null;
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }

    private void HandlePlayerDied()
    {
        ShowGameOver();
    }

    private void ApplyPauseState()
    {
        bool shouldPause = stopMenuOpen || gameOverOpen;
        Time.timeScale = shouldPause ? 0f : 1f;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        bool shouldShowCursor = stopMenuOpen || gameOverOpen;
        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetMenuVisible(GameObject root, bool visible)
    {
        if (root != null)
        {
            root.SetActive(visible);
        }
    }

    private void UpdatePartyometerUI()
    {
        if (partyometerSlider == null || GameManager.Instance == null)
        {
            return;
        }

        partyometerSlider.maxValue = GameManager.Instance.MaxPartyOMeter;
        partyometerSlider.value = GameManager.Instance.PartyOMeter;
    }
}
