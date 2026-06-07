using UnityEngine;

public class DJGhostFloatMotion : MonoBehaviour
{
    [SerializeField] private Vector3 localFloatOffset = new Vector3(0f, 0.35f, 0f);
    [SerializeField] private float floatAmplitude = 0.18f;
    [SerializeField] private float floatFrequency = 1.4f;
    [SerializeField] private float swayAmplitude = 0.12f;
    [SerializeField] private float swayFrequency = 0.9f;
    [SerializeField] private float rotationAmplitude = 6f;
    [SerializeField] private float rotationFrequency = 0.75f;

    private Vector3 startingLocalPosition;
    private Quaternion startingLocalRotation;

    private void Awake()
    {
        startingLocalPosition = transform.localPosition;
        startingLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        float swayOffset = Mathf.Cos(Time.time * swayFrequency) * swayAmplitude;
        float rotationOffset = Mathf.Sin(Time.time * rotationFrequency) * rotationAmplitude;

        transform.localPosition = startingLocalPosition + localFloatOffset + new Vector3(swayOffset, floatOffset, 0f);
        transform.localRotation = startingLocalRotation * Quaternion.Euler(0f, rotationOffset, 0f);
    }
}
