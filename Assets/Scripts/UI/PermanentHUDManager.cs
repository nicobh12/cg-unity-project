using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PermanentHUDManager : MonoBehaviour
{
    public static PermanentHUDManager Instance { get; private set; }

    [Header("Roots")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private GameObject healthRoot;

    [SerializeField] private GameObject partyometerRoot;
    [SerializeField] private GameObject bottleCounterRoot;

    [Header("Bars")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider partyometerSlider;

    [Header("Retry")]
    [SerializeField] private string retrySceneName = "nv1";

    private Health currentPlayerHealth;
    private InventorySystem currentPlayerInventory;
    private bool gameOverOpen;
    private bool cutsceneLocked;

    public bool IsGameplayLocked => gameOverOpen || cutsceneLocked;

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

        HideGameOver();
        SetCutsceneLock(false);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindCurrentPlayer();
    }

    public void ShowGameOver()
    {
        gameOverOpen = true;

        SetMenuVisible(healthRoot, false);
        SetMenuVisible(partyometerRoot, false);
        SetMenuVisible(bottleCounterRoot, false);

        SetMenuVisible(gameOverRoot, true);

        ApplyPauseState();
    }

    public void HideGameOver()
    {
        gameOverOpen = false;

        SetMenuVisible(healthRoot, true);
        SetMenuVisible(partyometerRoot, true);
        SetMenuVisible(bottleCounterRoot, true);

        SetMenuVisible(gameOverRoot, false);

        ApplyPauseState();
    }

    public void SetCutsceneLock(bool locked)
    {
        cutsceneLocked = locked;

        if (locked)
        {
            Time.timeScale = 0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        HideGameOver();

        gameOverOpen = false;
        cutsceneLocked = false;

        Time.timeScale = 1f;
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
        bool shouldPause = gameOverOpen || cutsceneLocked;
        Time.timeScale = shouldPause ? 0f : 1f;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        bool shouldShowCursor = gameOverOpen;
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

    public void RetryGame()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetRuntimeStateForRetry();
        }

        SceneManager.LoadScene(string.IsNullOrWhiteSpace(retrySceneName)
            ? SceneManager.GetActiveScene().name
            : retrySceneName);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        gameOverOpen = false;

        healthRoot.SetActive(false);
        partyometerRoot.SetActive(false);
        bottleCounterRoot.SetActive(false);
        hudRoot.SetActive(false);
        gameOverRoot.SetActive(false);

        SceneManager.LoadScene("Menu");
    }

    private void ForceCursorUnlocked()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
