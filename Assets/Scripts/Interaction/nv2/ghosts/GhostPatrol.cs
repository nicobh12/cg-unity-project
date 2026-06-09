using UnityEngine;

public class GhostPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 2f;

    private Transform target;

    private void Start()
    {
        target = pointB;
    }

    private void Update()
    {
        Vector3 direction = target.position - transform.position;

        // Girar hacia el destino
        if (direction != Vector3.zero)
        {
            transform.forward = direction.normalized;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }
}