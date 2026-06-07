using UnityEngine;

public class MusicWaveProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float lifetime = 8f;

    private Transform target;
    public bool IsSpecialWave { get; private set; }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 desiredDirection = (target.position - transform.position).normalized;
            if (desiredDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
            }
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void Initialize(Transform targetTransform, bool isSpecialWave, float speedMultiplier = 1f)
    {
        target = targetTransform;
        IsSpecialWave = isSpecialWave;
        speed *= Mathf.Max(0.1f, speedMultiplier);
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
}
