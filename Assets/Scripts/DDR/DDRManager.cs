using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class DDRManager : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private string songsResourcesFolder = "Songs";

    [Header("References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Camera ddrCamera;

    [Header("Animation")]
    [SerializeField] private string danceTriggerPrefix = "Dance";
    [SerializeField] private string missTriggerName = "MissedStep";

    [Header("Timing")]
    [SerializeField] private float stepDurationSeconds = 1.0f;
    [SerializeField] private float previewDurationSeconds = 0.5f;
    [SerializeField] private float inputDurationSeconds = 0.8f;
    [SerializeField] private float inputGraceSeconds = 0.10f;
    [SerializeField] private float postSongDelaySeconds = 3.0f;

    [Header("Debug")]
    [SerializeField] private bool verboseStepLogs = true;
    [SerializeField] private bool showDebugOverlay = true;

    [Header("UI")]
    [SerializeField] private TMP_Text songNameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject firstEntryTutorialPanel;
    [SerializeField] private bool allowKeyboardToCloseTutorial = true;
    [SerializeField] private KeyCode tutorialContinueKey = KeyCode.Return;

    //load current manager volume to restore later
    private float managerVolume;


    private static readonly KeyCode[] StepKeys =
    {
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.UpArrow,
        KeyCode.RightArrow,
        KeyCode.A,
        KeyCode.S,
        KeyCode.W,
        KeyCode.D
    };

    private Camera[] camerasToRestore;
    private DDRSongData activeSong;
    private AudioClip activeClip;
    private int currentStepIndex;
    private int totalMisses;
    private int totalHits;
    private double sequenceStartDspTime;
    private bool started;
    private bool finished;
    private bool returningToScene;
    private bool inputCapturedForCurrentStep;
    private bool waitingForTutorialClose;
    private bool hasStartedSong;
    private KeyCode capturedKeyForCurrentStep = KeyCode.None;
    private string lastStepDebug = string.Empty;
    private string songStatus = "Loading...";
    private float cachedTimeScale = 1f;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        ActivateDDRCamera();

        //stop music manager music if it's playing
        if (MusicManager.Instance != null)        {
            managerVolume = MusicManager.Instance.GetVolume();
            MusicManager.Instance.SetVolume(0f);
        }

        if (ShouldShowFirstEntryTutorial())
        {
            BeginFirstEntryTutorialGate();
            return;
        }

        StartSongIfNeeded();
    }

    private void OnDestroy()
    {
        if (waitingForTutorialClose)
        {
            Time.timeScale = cachedTimeScale;
        }

        RestoreCameras();
    }

    private void Update()
    {
        if (waitingForTutorialClose)
        {
            if (allowKeyboardToCloseTutorial && Input.GetKeyDown(tutorialContinueKey))
            {
                OnTutorialContinuePressed();
            }

            return;
        }

        if (!started || finished || activeSong == null || activeSong.steps == null)
        {
            return;
        }

        RunStepTimeline();

        if (currentStepIndex >= activeSong.steps.Length)
        {
            FinishSong();
        }
    }

    private IEnumerator BeginRandomSongRoutine()
    {
        TextAsset[] charts = Resources.LoadAll<TextAsset>(songsResourcesFolder);
        if (charts == null || charts.Length == 0)
        {
            Debug.LogError("No DDR chart JSON files found in Resources/Songs.");
            yield break;
        }

        TextAsset selectedChart = charts[Random.Range(0, charts.Length)];
        activeSong = JsonUtility.FromJson<DDRSongData>(selectedChart.text);

        if (activeSong == null || activeSong.steps == null || activeSong.steps.Length == 0)
        {
            Debug.LogError("Selected DDR chart is invalid or has no steps.");
            yield break;
        }

        activeClip = Resources.Load<AudioClip>($"{songsResourcesFolder}/{activeSong.audioFileName}");
        if (activeClip == null)
        {
            Debug.LogError($"Could not load audio clip '{activeSong.audioFileName}' from Resources/{songsResourcesFolder}.");
            yield break;
        }

        currentStepIndex = 0;
        totalMisses = 0;
        totalHits = 0;
        inputCapturedForCurrentStep = false;
        capturedKeyForCurrentStep = KeyCode.None;
        songStatus = $"Ready: {activeSong.songId}";

        if (songNameText != null)
        {
            songNameText.text = activeSong.songId;
        }

        UpdateScoreUI();

        if (resultText != null)
        {
            resultText.text = "";
        }

        if (verboseStepLogs)
        {
            Debug.Log($"DDR Song Selected: {activeSong.songId} | BPM: {activeSong.bpm} | Steps: {activeSong.steps.Length}");
            Debug.Log($"DDR Step Timing: step={stepDurationSeconds:F1}s, preview={previewDurationSeconds:F1}s, input={inputDurationSeconds:F1}s, grace={inputGraceSeconds:F2}s");
        }

        audioSource.clip = activeClip;
        sequenceStartDspTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(sequenceStartDspTime);

        started = true;
        songStatus = $"Playing: {activeSong.songId}";

        yield return null;
    }

    private bool ShouldShowFirstEntryTutorial()
    {
        return false;
    }

    private void BeginFirstEntryTutorialGate()
    {
        if (firstEntryTutorialPanel == null)
        {
            Debug.LogWarning("DDR first-entry tutorial panel is not assigned. Starting song immediately.");
            StartSongIfNeeded();
            return;
        }

        waitingForTutorialClose = true;
        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        songStatus = "Tutorial";
        firstEntryTutorialPanel.SetActive(true);
    }

    private void StartSongIfNeeded()
    {
        if (hasStartedSong)
        {
            return;
        }

        hasStartedSong = true;
        StartCoroutine(BeginRandomSongRoutine());
    }

    public void OnTutorialContinuePressed()
    {
        if (!waitingForTutorialClose)
        {
            return;
        }

        waitingForTutorialClose = false;

        if (firstEntryTutorialPanel != null)
        {
            firstEntryTutorialPanel.SetActive(false);
        }

        Time.timeScale = cachedTimeScale;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkDDRTutorialAsSeen();
        }

        StartSongIfNeeded();
    }

    private void RunStepTimeline()
    {
        if (currentStepIndex >= activeSong.steps.Length)
        {
            return;
        }

        float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
        float safePreviewDuration = Mathf.Clamp(previewDurationSeconds, 0f, safeStepDuration);
        float safeInputDuration = Mathf.Clamp(inputDurationSeconds, 0.1f, safeStepDuration - safePreviewDuration);
        double timelineTime = AudioSettings.dspTime - sequenceStartDspTime;

        while (currentStepIndex < activeSong.steps.Length)
        {
            double stepStartTime = currentStepIndex * safeStepDuration;
            double elapsed = timelineTime - stepStartTime;

            if (elapsed < 0f)
            {
                return;
            }

            DDRStepData step = activeSong.steps[currentStepIndex];

            if (elapsed < safePreviewDuration)
            {
                songStatus = $"Step {currentStepIndex + 1}: Preview {step.key}";
                return;
            }

            if (elapsed < safePreviewDuration + safeInputDuration + inputGraceSeconds)
            {
                songStatus = inputCapturedForCurrentStep
                    ? $"Step {currentStepIndex + 1}: Input locked"
                    : $"Step {currentStepIndex + 1}: PRESS {step.key}";

                if (!inputCapturedForCurrentStep && TryGetArrowKeyDown(out KeyCode pressedKey))
                {
                    inputCapturedForCurrentStep = true;
                    capturedKeyForCurrentStep = pressedKey;
                    ResolveCurrentStep(step);
                    currentStepIndex++;
                    inputCapturedForCurrentStep = false;
                    capturedKeyForCurrentStep = KeyCode.None;
                    continue;
                }

                return;
            }

            ResolveCurrentStep(step);
            currentStepIndex++;
            inputCapturedForCurrentStep = false;
            capturedKeyForCurrentStep = KeyCode.None;
        }
    }

    private bool TryGetArrowKeyDown(out KeyCode pressedKey)
    {
        foreach (KeyCode key in StepKeys)
        {
            if (Input.GetKeyDown(key))
            {
                pressedKey = key;
                return true;
            }
        }

        pressedKey = KeyCode.None;
        return false;
    }

    private void ResolveCurrentStep(DDRStepData step)
    {
        KeyCode expectedKey = ParseStepKey(step.key);

        if (expectedKey == KeyCode.None)
        {
            RegisterMiss();
            lastStepDebug = $"MISS invalid key {step.key}";
        }
        else if (capturedKeyForCurrentStep == expectedKey)
        {
            RegisterHit(step.animationValue);
            lastStepDebug = $"HIT step {currentStepIndex + 1}";
        }
        else
        {
            RegisterMiss();
            lastStepDebug = inputCapturedForCurrentStep
                ? $"MISS wrong key"
                : $"MISS timeout step {currentStepIndex + 1}";
        }

        if (verboseStepLogs)
        {
            Debug.Log(lastStepDebug);
        }
    }

    private void RegisterHit(int animationValue)
    {
        totalHits++;

        UpdateScoreUI();
        ShowResult("HIT");

        if (playerAnimator != null)
        {
            string danceTriggerName = $"{danceTriggerPrefix}{animationValue}";
            playerAnimator.SetTrigger(danceTriggerName);

            if (verboseStepLogs)
            {
                Debug.Log($"Set animator trigger '{danceTriggerName}'");
            }
        }
    }

    private void RegisterMiss()
    {
        totalMisses += 1;
        UpdateScoreUI();
        ShowResult("MISS");
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(missTriggerName);

            if (verboseStepLogs)
            {
                Debug.Log($"Set animator trigger '{missTriggerName}'");
            }
        }
    }

    private Coroutine resultRoutine;

    private void FinishSong()
    {
        if (finished)
        {
            return;
        }

        finished = true;
        float totalSteps = Mathf.Max(1, totalHits + totalMisses);
        float missRatio = totalMisses / totalSteps;
        songStatus = $"Finished: {activeSong.songId} (Miss Ratio {missRatio:P0})";

        if (verboseStepLogs)
        {
            Debug.Log(songStatus);
        }

        if (!returningToScene)
        {
            StartCoroutine(ReturnAfterDelay(missRatio));
        }
    }

    private IEnumerator ReturnAfterDelay(float missRatio)
    {
        returningToScene = true;
        yield return new WaitForSeconds(postSongDelaySeconds);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(managerVolume);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteDDRChallenge(missRatio);
            yield break;
        }
    }

    private KeyCode ParseStepKey(string keyName)
    {
        switch (keyName)
        {
            case "Left":
                return KeyCode.LeftArrow;
            case "Down":
                return KeyCode.DownArrow;
            case "Up":
                return KeyCode.UpArrow;
            case "Right":
                return KeyCode.RightArrow;
            case "A":
                return KeyCode.A;
            case "S":
                return KeyCode.S;
            case "W":
                return KeyCode.W;
            case "D":
                return KeyCode.D;
            default:
                return KeyCode.None;
        }
    }

    private void ActivateDDRCamera()
    {
        if (ddrCamera == null)
        {
            return;
        }

        camerasToRestore = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cameraInstance in camerasToRestore)
        {
            if (cameraInstance != ddrCamera)
            {
                cameraInstance.enabled = false;
            }
        }

        ddrCamera.enabled = true;
    }

    private void RestoreCameras()
    {
        if (camerasToRestore == null)
        {
            return;
        }

        foreach (Camera cameraInstance in camerasToRestore)
        {
            if (cameraInstance != null)
            {
                cameraInstance.enabled = true;
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null)
            return;

        int totalSteps = activeSong != null ? activeSong.steps.Length : 0;

        scoreText.text = $"{totalHits}/{totalSteps}";
    }

    private void ShowResult(string result)
    {
        if (resultText == null)
            return;

        if (resultRoutine != null)
        {
            StopCoroutine(resultRoutine);
        }

        resultRoutine = StartCoroutine(ShowResultRoutine(result));
    }

    private IEnumerator ShowResultRoutine(string result)
    {
        resultText.text = result;

        yield return new WaitForSeconds(0.6f);

        resultText.text = "";
    }

/*     private void OnGUI()
    {
        if (!showDebugOverlay)
        {
            return;
        }

        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(16f, 16f, 560f, 260f), GUI.skin.box);
        GUILayout.Label("DDR DEBUG");
        GUILayout.Label(songStatus);

        if (activeSong != null)
        {
            int totalSteps = activeSong.steps != null ? activeSong.steps.Length : 0;
            float missRatio = totalSteps > 0 ? (float)totalMisses / totalSteps : 0f;

            GUILayout.Label($"Song: {activeSong.songId}");
            GUILayout.Label($"Step: {Mathf.Min(currentStepIndex + 1, Mathf.Max(1, totalSteps))}/{Mathf.Max(1, totalSteps)}");
            GUILayout.Label($"Hits: {totalHits} | Misses: {totalMisses} | Miss Ratio: {missRatio:P1}");

            if (HasActiveSong && currentStepIndex < activeSong.steps.Length)
            {
                DDRStepData step = activeSong.steps[currentStepIndex];
                float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
                float safePreviewDuration = Mathf.Clamp(previewDurationSeconds, 0f, safeStepDuration);
                double timelineTime = AudioSettings.dspTime - sequenceStartDspTime;
                double stepStart = currentStepIndex * safeStepDuration;
                double stepElapsed = timelineTime - stepStart;
                bool inInputWindow = stepElapsed >= safePreviewDuration;
                float phaseRemaining = inInputWindow
                    ? Mathf.Max(0f, safeStepDuration - (float)stepElapsed)
                    : Mathf.Max(0f, safePreviewDuration - (float)stepElapsed);

                GUILayout.Label($"Next Key: {step.key} | Anim Value: {step.animationValue}");
                GUILayout.Label(inInputWindow
                    ? $"Phase: INPUT ({phaseRemaining:F2}s left)"
                    : $"Phase: PREVIEW ({phaseRemaining:F2}s left)");
                GUILayout.Label(inputCapturedForCurrentStep
                    ? $"Captured Input: {capturedKeyForCurrentStep}"
                    : "Captured Input: none");
            }
        }

        GUILayout.Label($"Last Step: {lastStepDebug}");
        GUILayout.EndArea();
    }*/

    public bool HasActiveSong => started && !finished && activeSong != null && activeSong.steps != null;
    public int CurrentStepIndex => currentStepIndex;
    public int TotalSteps => activeSong != null && activeSong.steps != null ? activeSong.steps.Length : 0;
    public string CurrentStepKey => HasActiveSong && currentStepIndex < activeSong.steps.Length ? activeSong.steps[currentStepIndex].key : string.Empty;
    public string CurrentStepDanceTriggerName => HasActiveSong && currentStepIndex < activeSong.steps.Length ? $"{danceTriggerPrefix}{activeSong.steps[currentStepIndex].animationValue}" : string.Empty;

    public bool IsPreviewPhase
    {
        get
        {
            if (!HasActiveSong || currentStepIndex >= activeSong.steps.Length)
            {
                return false;
            }

            double timelineTime = AudioSettings.dspTime - sequenceStartDspTime;
            float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
            float safePreviewDuration = Mathf.Clamp(previewDurationSeconds, 0f, safeStepDuration);
            double stepStart = currentStepIndex * safeStepDuration;
            double elapsed = timelineTime - stepStart;
            return elapsed >= 0f && elapsed < safePreviewDuration;
        }
    }

    public bool IsInputPhase
    {
        get
        {
            if (!HasActiveSong || currentStepIndex >= activeSong.steps.Length)
            {
                return false;
            }

            double timelineTime = AudioSettings.dspTime - sequenceStartDspTime;
            float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
            float safePreviewDuration = Mathf.Clamp(previewDurationSeconds, 0f, safeStepDuration);
            float safeInputDuration = Mathf.Clamp(inputDurationSeconds, 0.1f, safeStepDuration - safePreviewDuration);
            double stepStart = currentStepIndex * safeStepDuration;
            double elapsed = timelineTime - stepStart;
            return elapsed >= safePreviewDuration && elapsed < safePreviewDuration + safeInputDuration + inputGraceSeconds;
        }
    }

    public bool HasCapturedInputForCurrentStep => inputCapturedForCurrentStep;
    public float CurrentStepElapsedSeconds
    {
        get
        {
            if (!HasActiveSong || currentStepIndex >= activeSong.steps.Length)
            {
                return 0f;
            }

            float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
            double timelineTime = AudioSettings.dspTime - sequenceStartDspTime;
            double stepStart = currentStepIndex * safeStepDuration;
            return Mathf.Max(0f, (float)(timelineTime - stepStart));
        }
    }

    public float CurrentStepPreviewProgress
    {
        get
        {
            float safeStepDuration = Mathf.Max(0.1f, stepDurationSeconds);
            float safePreviewDuration = Mathf.Clamp(previewDurationSeconds, 0f, safeStepDuration);
            if (safePreviewDuration <= 0f)
            {
                return 1f;
            }

            return Mathf.Clamp01(CurrentStepElapsedSeconds / safePreviewDuration);
        }
    }

    
}
