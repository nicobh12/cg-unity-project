using System.Collections;
using UnityEngine;

public class DJGhostBossAttack : MonoBehaviour
{
    [SerializeField] private MusicWaveProjectile regularWavePrefab;
    [SerializeField] private MusicWaveProjectile specialWavePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float regularWaveCooldown = 2f;
    [SerializeField] private float specialWaveCooldown = 6f;
    [SerializeField] private float specialWaveSpeedMultiplier = 1.35f;

    private Coroutine attackRoutine;

    private void OnEnable()
    {

        Debug.Log("DJGhostBossAttack enabled, starting attack loop.");

        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        if (playerTarget == null)
        {
            Debug.LogWarning("Player target not assigned for DJGhostBossAttack. Attempting to find player by tag.");

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
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
        Debug.Log("Starting DJGhostBoss attack loop.");

        while (true)
        {
            SpawnRegularWave();
            yield return new WaitForSeconds(regularWaveCooldown);

            SpawnSpecialWave();
            yield return new WaitForSeconds(specialWaveCooldown);
        }
    }

    private void SpawnRegularWave()
    {
        Debug.Log("Spawning regular music wave.");
        
        if (regularWavePrefab == null || playerTarget == null)
        {
            return;
        }

        MusicWaveProjectile wave = Instantiate(regularWavePrefab, spawnPoint.position, Quaternion.identity);
        wave.Initialize(playerTarget, false, 1f);
    }

    private void SpawnSpecialWave()
    {
        Debug.Log("Spawning special music wave.");

        if (specialWavePrefab == null || playerTarget == null)
        {
            return;
        }

        MusicWaveProjectile wave = Instantiate(specialWavePrefab, spawnPoint.position, Quaternion.identity);
        wave.Initialize(playerTarget, true, specialWaveSpeedMultiplier);
    }
}
