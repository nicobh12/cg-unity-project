using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerWaveDamageReceiver : MonoBehaviour
{
    [SerializeField] private float regularWaveDamage = 10f;
    [SerializeField] private float specialWaveDamage = 10f;
    [SerializeField] private int maxSpecialHitsBeforeDefeat = 3;

    private Health health;
    private int specialWaveHits;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleWaveHit(other.GetComponent<MusicWaveProjectile>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleWaveHit(collision.gameObject.GetComponent<MusicWaveProjectile>());
    }

    private void TryHandleWaveHit(MusicWaveProjectile wave)
    {
        if (wave == null || health == null || health.IsDead)
        {
            return;
        }

        if (wave.IsSpecialWave)
        {
            specialWaveHits += 1;
            health.ApplyDamage(specialWaveDamage);

            if (specialWaveHits > maxSpecialHitsBeforeDefeat)
            {
                health.Deplete();
            }
        }
        else
        {
            health.ApplyDamage(regularWaveDamage);
        }

        wave.Consume();
    }
}
