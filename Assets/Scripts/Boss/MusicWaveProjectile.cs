using UnityEngine;

public class MusicWaveProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float lifetime = 8f;

    private Vector3 direction;

    public bool IsSpecialWave { get; private set; }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void Initialize(Vector3 targetPosition, bool isSpecialWave, float speedMultiplier = 1f)
    {
        IsSpecialWave = isSpecialWave;

        speed *= Mathf.Max(0.1f, speedMultiplier);

        direction = (targetPosition - transform.position).normalized;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
}