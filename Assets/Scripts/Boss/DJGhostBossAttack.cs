using System.Collections;
using UnityEngine;

public class DJGhostBossAttack : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private MusicWaveProjectile regularWavePrefab;
    [SerializeField] private MusicWaveProjectile specialWavePrefab;

    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerTarget;

    [Header("Attack Timing")]
    [SerializeField] private float minAttackDelay = 2.5f;
    [SerializeField] private float maxAttackDelay = 5.5f;

    [Header("Attack Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float specialAttackChance = 0.4f;

    [Header("Special Attack")]
    [SerializeField] private float specialWaveSpeedMultiplier = 1.35f;

    private Coroutine attackRoutine;

    private void OnEnable()
    {
        Debug.Log("DJGhostBossAttack enabled.");

        if (spawnPoint == null)
            spawnPoint = transform;

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerTarget = player.transform;
        }

        attackRoutine = StartCoroutine(AttackLoop());
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator AttackLoop()
    {
            while (GameManager.Instance == null || !GameManager.Instance.CombatEnded)
            {
                float waitTime = Random.Range(minAttackDelay, maxAttackDelay);
                yield return new WaitForSeconds(waitTime);

                PerformRandomAttack();
            }
    }

    private void PerformRandomAttack()
    {
        if (playerTarget == null)
            return;

        bool useSpecial = Random.value < specialAttackChance;

        Vector3 targetPosition = playerTarget.position;

        if (useSpecial)
        {
            SpawnSpecialWave(targetPosition);
        }
        else
        {
            SpawnRegularWave(targetPosition);
        }
    }

    private void SpawnRegularWave(Vector3 targetPosition)
    {
        if (regularWavePrefab == null)
            return;

        Debug.Log("Regular Attack");

        MusicWaveProjectile wave =
            Instantiate(regularWavePrefab, spawnPoint.position, Quaternion.identity);

        wave.Initialize(targetPosition, false, 1f);
    }

    private void SpawnSpecialWave(Vector3 targetPosition)
    {
        if (specialWavePrefab == null)
            return;

        Debug.Log("Special Attack");

        MusicWaveProjectile wave =
            Instantiate(specialWavePrefab, spawnPoint.position, Quaternion.identity);

        wave.Initialize(targetPosition, true, specialWaveSpeedMultiplier);
    }
}