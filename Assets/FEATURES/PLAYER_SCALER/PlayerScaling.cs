using UnityEngine;
using TMPro;

public class PlayerScaling : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("Drag the player GameObject here.")]
    [SerializeField] private Transform playerTransform;

    [Header("Scaling Settings")]
    [Tooltip("The rate at which the player's scale changes.")]
    [SerializeField] private float scalingRate = 0.1f;
    [Tooltip("The minimum scale allowed for the player (50%).")]
    [SerializeField] private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // 50% scale
    [Tooltip("The maximum scale allowed for the player (150%).")]
    [SerializeField] private Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f); // 150% scale

    [Header("UI Feedback")]
    [Tooltip("TextMeshPro object to display the player's current scale.")]
    [SerializeField] private TextMeshProUGUI scaleDisplay;

    private void Awake()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned. Please assign it in the Inspector.");
        }

        if (scaleDisplay == null)
        {
            Debug.LogError("Scale Display TMP is not assigned. Please assign it in the Inspector.");
        }

        UpdateScaleDisplay(); // Initialize display
    }

    public void ScaleUp()
    {
        if (playerTransform == null) return;

        Vector3 newScale = playerTransform.localScale + Vector3.one * scalingRate;
        playerTransform.localScale = ClampScale(newScale);
        UpdateScaleDisplay();
    }

    public void ScaleDown()
    {
        if (playerTransform == null) return;

        Vector3 newScale = playerTransform.localScale - Vector3.one * scalingRate;
        playerTransform.localScale = ClampScale(newScale);
        UpdateScaleDisplay();
    }

    private Vector3 ClampScale(Vector3 scale)
    {
        return new Vector3(
            Mathf.Clamp(scale.x, minScale.x, maxScale.x),
            Mathf.Clamp(scale.y, minScale.y, maxScale.y),
            Mathf.Clamp(scale.z, minScale.z, maxScale.z)
        );
    }

    private void UpdateScaleDisplay()
    {
        if (scaleDisplay != null)
        {
            // Assuming uniform scaling, convert X scale to percentage
            float currentScalePercentage = playerTransform.localScale.x * 100f; // Convert to percentage
            scaleDisplay.text = $"{currentScalePercentage:F0}%"; // Display as whole number
        }
    }
}
