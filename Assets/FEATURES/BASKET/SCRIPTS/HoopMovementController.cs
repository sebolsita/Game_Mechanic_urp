using UnityEngine;
using TMPro;

public class HoopMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The rate at which the hoop moves up or down.")]
    [SerializeField] private float movementStep = 0.5f;

    [Tooltip("The minimum height for the hoop.")]
    [SerializeField] private float minHeight = 1.0f;

    [Tooltip("The maximum height for the hoop.")]
    [SerializeField] private float maxHeight = 5.0f;

    [Header("UI Feedback")]
    [Tooltip("TextMeshPro object to display the hoop's current height.")]
    [SerializeField] private TextMeshProUGUI heightDisplay;

    private void Start()
    {
        UpdateHeightDisplay(); // Initialize the display with the current height
    }

    public void MoveUp()
    {
        Vector3 newPosition = transform.position + Vector3.up * movementStep;
        transform.position = ClampHeight(newPosition);
        UpdateHeightDisplay();
    }

    public void MoveDown()
    {
        Vector3 newPosition = transform.position - Vector3.up * movementStep;
        transform.position = ClampHeight(newPosition);
        UpdateHeightDisplay();
    }

    private Vector3 ClampHeight(Vector3 position)
    {
        // Clamp the Y value to ensure the hoop stays within the allowed height range
        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);
        return position;
    }

    private void UpdateHeightDisplay()
    {
        if (heightDisplay != null)
        {
            // Display the current Y position of the hoop as height
            heightDisplay.text = $"Height: {transform.position.y:F2}m";
        }
    }
}
