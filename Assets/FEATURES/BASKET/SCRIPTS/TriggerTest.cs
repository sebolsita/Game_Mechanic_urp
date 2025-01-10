using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    [SerializeField] private Collider _targetCollider; // Assign the collider in the Inspector.

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected: {other.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger exited: {other.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");
    }
}
