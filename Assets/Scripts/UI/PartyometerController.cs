using UnityEngine;
using UnityEngine.UI;

public class PartyometerController : MonoBehaviour
{
    public static PartyometerController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Slider partyometerSlider;

    // ❌ ya no se asignan en Inspector (pueden no existir en escena)
    private LightController lightController;
    private SpeakerManager speakerManager;

    [Header("Values")]
    [SerializeField] private float fullValue = 100f;
    [SerializeField] private float completedValue = 50f;

    private bool forceZero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        BindSceneObjects();
        ApplyValue(fullValue);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        BindSceneObjects();
    }

    private void BindSceneObjects()
    {
        lightController = FindFirstObjectByType<LightController>();
        speakerManager = FindFirstObjectByType<SpeakerManager>();
    }

    private void Update()
    {
        if (forceZero)
        {
            ApplyValue(0f);
            return;
        }

        ApplyValue(CalculatePartyometerValue());
    }

    public void ForceToZero()
    {
        forceZero = true;
        ApplyValue(0f);
    }

    public void ResetToFull()
    {
        forceZero = false;
        ApplyValue(fullValue);
    }

    private float CalculatePartyometerValue()
    {
        int totalTargets = 0;
        int completedTargets = 0;

        // 💡 LIGHTS
        if (lightController != null && lightController.lights != null)
        {
            foreach (LightSwitch lightSwitch in lightController.lights)
            {
                if (lightSwitch == null) continue;

                totalTargets++;

                if (lightSwitch.IsOn)
                    completedTargets++;
            }
        }

        // 🔊 SPEAKERS
        if (speakerManager != null && speakerManager.speakers != null)
        {
            foreach (Speaker speaker in speakerManager.speakers)
            {
                if (speaker == null) continue;

                totalTargets++;

                if (speaker.IsBroken)
                    completedTargets++;
            }
        }

        if (totalTargets <= 0)
            return fullValue;

        float progress = Mathf.Clamp01((float)completedTargets / totalTargets);
        return Mathf.Lerp(fullValue, completedValue, progress);
    }

    private void ApplyValue(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0f, fullValue);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPartyOMeter(clampedValue);
        }

        if (partyometerSlider != null)
        {
            partyometerSlider.maxValue = fullValue;
            partyometerSlider.value = clampedValue;
        }
    }
}