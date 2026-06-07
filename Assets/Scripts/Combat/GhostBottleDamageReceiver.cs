using UnityEngine;

[RequireComponent(typeof(Health))]
public class GhostBottleDamageReceiver : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleBottleHit(other.GetComponent<BottleProjectile>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleBottleHit(collision.gameObject.GetComponent<BottleProjectile>());
    }

    private void TryHandleBottleHit(BottleProjectile bottleProjectile)
    {
        if (bottleProjectile == null || health == null || health.IsDead)
        {
            return;
        }

        health.ApplyDamage(bottleProjectile.Damage);
        bottleProjectile.ConsumeOnHit();
    }
}
