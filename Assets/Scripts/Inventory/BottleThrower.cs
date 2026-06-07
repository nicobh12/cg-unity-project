using UnityEngine;

public class BottleThrower : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private ItemDefinition bottleItem;
    [SerializeField] private GameObject bottlePrefab;
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private Transform aimPoint;
    [SerializeField] private ThrowAimIndicator throwAimIndicator;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse0;
    [SerializeField] private bool throwOnKeyRelease = true;
    [SerializeField] private float throwForce = 14f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float maxAimDistance = 25f;
    [SerializeField] private float bottleDamage = 20f;

    private void Awake()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GetComponentInParent<InventorySystem>();
        }

        if (throwOrigin == null)
        {
            throwOrigin = transform;
        }

        if (throwAimIndicator == null)
        {
            throwAimIndicator = GetComponent<ThrowAimIndicator>();
        }
    }

    private void Update()
    {
        bool shouldThrow = throwOnKeyRelease ? Input.GetKeyUp(throwKey) : Input.GetKeyDown(throwKey);
        if (!shouldThrow)
        {
            return;
        }

        ThrowBottle();
    }

    public bool ThrowBottle()
    {
        if (inventorySystem == null || bottleItem == null || bottlePrefab == null || throwOrigin == null)
        {
            return false;
        }

        if (!inventorySystem.ConsumeItem(bottleItem.itemId, 1))
        {
            return false;
        }

        Vector3 targetPoint = GetTargetPoint();
        Vector3 throwDirection = (targetPoint - throwOrigin.position).normalized;
        if (throwDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            throwDirection = throwOrigin.forward;
        }

        GameObject bottleInstance = Instantiate(bottlePrefab, throwOrigin.position, Quaternion.LookRotation(throwDirection));
        AttachBottleCombatData(bottleInstance);

        if (bottleInstance.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.AddForce(throwDirection * throwForce + Vector3.up * upwardForce, ForceMode.VelocityChange);
        }

        return true;
    }

    private Vector3 GetTargetPoint()
    {
        if (throwAimIndicator != null)
        {
            return throwAimIndicator.CurrentAimPoint;
        }

        if (aimPoint != null)
        {
            return aimPoint.position;
        }

        Ray ray = new Ray(throwOrigin.position, throwOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
        {
            return hit.point;
        }

        return throwOrigin.position + throwOrigin.forward * maxAimDistance;
    }

    private void AttachBottleCombatData(GameObject bottleInstance)
    {
        BottleProjectile bottleProjectile = bottleInstance.GetComponent<BottleProjectile>();
        if (bottleProjectile == null)
        {
            bottleProjectile = bottleInstance.AddComponent<BottleProjectile>();
        }

        bottleProjectile.SetDamage(bottleDamage);
    }
}
