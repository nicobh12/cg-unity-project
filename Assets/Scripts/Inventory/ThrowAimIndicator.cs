using UnityEngine;

public class ThrowAimIndicator : MonoBehaviour
{
    [SerializeField] private Transform aimSource;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse0;
    [SerializeField] private float maxAimDistance = 25f;
    [SerializeField] private LayerMask aimMask = ~0;
    [SerializeField] private GameObject indicatorVisual;

    public Vector3 CurrentAimPoint { get; private set; }
    public bool IsAiming { get; private set; }

    private void Awake()
    {
        if (aimSource == null)
        {
            aimSource = transform;
        }

        if (indicatorVisual != null)
        {
            indicatorVisual.SetActive(false);
        }
    }

    private void Update()
    {
        IsAiming = Input.GetKey(aimKey);
        CurrentAimPoint = CalculateAimPoint();

        if (indicatorVisual == null)
        {
            return;
        }

        indicatorVisual.SetActive(IsAiming);
        if (IsAiming)
        {
            indicatorVisual.transform.position = CurrentAimPoint;
        }
    }

    private Vector3 CalculateAimPoint()
    {
        Ray ray = new Ray(aimSource.position, aimSource.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return aimSource.position + aimSource.forward * maxAimDistance;
    }
}
