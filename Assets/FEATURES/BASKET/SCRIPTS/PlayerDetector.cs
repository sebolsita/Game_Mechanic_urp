using UnityEngine;

public class ZoneDetector : MonoBehaviour
{
    public delegate void ZoneEvent(int zoneScore); // Event for zone detection
    public static event ZoneEvent OnPlayerEnterZone; // Triggered when the player enters this zone

    [SerializeField] private int _zoneScore; // Score value for this zone (e.g., 1, 2, or 3)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered Zone {_zoneScore}.");
            OnPlayerEnterZone?.Invoke(_zoneScore); // Notify the main script
        }
    }
}
