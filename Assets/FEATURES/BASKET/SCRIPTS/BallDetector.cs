using UnityEngine;

public class HoopColliderDetector : MonoBehaviour
{
    public delegate void HoopEvent(string colliderName, bool isExiting);
    public static event HoopEvent OnBallHoopEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log($"Ball entered hoop collider: {gameObject.name}");
            OnBallHoopEvent?.Invoke(gameObject.name, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log($"Ball exited hoop collider: {gameObject.name}");
            OnBallHoopEvent?.Invoke(gameObject.name, true);
        }
    }
}

