using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NV3FinaleSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health djBossHealth;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DJGhostBossAttack bossAttack;
    [SerializeField] private GameObject ghostVisualRoot;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera gameplayCamera;
    [SerializeField] private CinemachineCamera ghostCamera;
    [SerializeField] private int gameplayCameraPriority = 10;
    [SerializeField] private int ghostCameraPriority = 30;

    [Header("Sequence")]
    [SerializeField] private float revealDelay = 2f;
    [SerializeField] private float vanishDelay = 1.5f;
    [SerializeField] private float creditsDelay = 0.75f;
    [SerializeField] private string creditsSceneName = "credits";

    private bool sequenceStarted;

    private void Awake()
    {
        if (djBossHealth == null)
        {
            djBossHealth = FindAnyObjectByType<Health>();
        }
    }

    private void OnEnable()
    {
        if (djBossHealth != null)
        {
            djBossHealth.onDied.AddListener(StartSequence);
        }
    }

    private void Start()
    {
        if (djBossHealth != null && djBossHealth.IsDead)
        {
            StartSequence();
        }
    }

    private void OnDisable()
    {
        if (djBossHealth != null)
        {
            djBossHealth.onDied.RemoveListener(StartSequence);
        }
    }

    public void StartSequence()
    {
        if (sequenceStarted)
        {
            return;
        }

        sequenceStarted = true;
        StartCoroutine(PlaySequenceRoutine());
    }

    private IEnumerator PlaySequenceRoutine()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPartyOMeter(0f);
        }

        if (PartyometerController.Instance != null)
        {
            PartyometerController.Instance.ForceToZero();
        }

        if (PermanentHUDManager.Instance != null)
        {
            PermanentHUDManager.Instance.SetCutsceneLock(true);
        }

        if (playerController != null)
        {
            playerController.desactivarMoviemiento();
        }

        if (bossAttack != null)
        {
            bossAttack.enabled = false;
        }

        if (ghostCamera != null)
        {
            ghostCamera.Priority = ghostCameraPriority;
        }

        if (gameplayCamera != null)
        {
            gameplayCamera.Priority = gameplayCameraPriority;
        }

        if (ghostVisualRoot != null)
        {
            ghostVisualRoot.SetActive(true);
        }

        yield return new WaitForSeconds(revealDelay);
        yield return new WaitForSeconds(vanishDelay);

        if (ghostVisualRoot != null)
        {
            ghostVisualRoot.SetActive(false);
        }

        yield return new WaitForSeconds(creditsDelay);

        if (!string.IsNullOrWhiteSpace(creditsSceneName))
        {
            SceneManager.LoadScene(creditsSceneName);
        }
    }
}
