using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerWaveDamageReceiver : MonoBehaviour
{
    [SerializeField] private float regularWaveDamage = 10f;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleWaveHit(other.GetComponentInParent<MusicWaveProjectile>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleWaveHit(collision.gameObject.GetComponentInParent<MusicWaveProjectile>());
    }

    private void TryHandleWaveHit(MusicWaveProjectile wave)
    {
        if (wave == null || health == null || health.IsDead)
            return;

        if (GameManager.Instance != null && GameManager.Instance.CombatEnded)
            return;

        if (wave.IsSpecialWave)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartDDRChallengeFromSpecialWave();
            }
        }
        else
        {
            health.ApplyDamage(regularWaveDamage);
        }

        wave.Consume();
    }
}
