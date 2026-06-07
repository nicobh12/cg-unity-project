using UnityEngine;

public class BottleProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float maxLifetime = 10f;

    public float Damage => damage;

    public void SetDamage(float value)
    {
        damage = Mathf.Max(0f, value);
    }

    private void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    public void ConsumeOnHit()
    {
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
