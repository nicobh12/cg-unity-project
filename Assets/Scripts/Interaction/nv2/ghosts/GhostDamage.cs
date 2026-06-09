using System.Collections;
using UnityEngine;

public class GhostDamage : MonoBehaviour
{
    public float damage = 10f;
    public float cooldown = 2f;

    private bool canDamage = true;

    private void OnTriggerStay(Collider other)
    {
        if (!canDamage)
            return;

        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.ApplyDamage(damage);

            StartCoroutine(DamageCooldown());
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;

        yield return new WaitForSeconds(cooldown);

        canDamage = true;
    }
}