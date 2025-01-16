using UnityEngine;
using TMPro;

public class GravityController : MonoBehaviour
{
    [Header("Gravity Settings")]
    [Tooltip("The rate at which gravity increases or decreases.")]
    [SerializeField] private float gravityStep = 1.0f;

    [Tooltip("The minimum gravity value.")]
    [SerializeField] private float minGravity = -50.0f;

    [Tooltip("The maximum gravity value.")]
    [SerializeField] private float maxGravity = -1.0f;

    [Tooltip("Initial gravity value on game start.")]
    [SerializeField] private float initialGravity = -9.81f;

    [Header("UI Feedback")]
    [Tooltip("TextMeshPro object to display the current gravity in g's.")]
    [SerializeField] private TextMeshProUGUI gravityDisplay;

    private const float EarthGravity = -9.81f; // Reference value for 1 g

    private void Start()
    {
        // Set the initial gravity value
        Physics.gravity = new Vector3(0, initialGravity, 0);
        UpdateGravityDisplay(); // Initialize display
    }

    public void IncreaseGravity()
    {
        float newGravity = Mathf.Clamp(Physics.gravity.y - gravityStep, minGravity, maxGravity);
        Physics.gravity = new Vector3(0, newGravity, 0);
        UpdateGravityDisplay();
    }

    public void DecreaseGravity()
    {
        float newGravity = Mathf.Clamp(Physics.gravity.y + gravityStep, minGravity, maxGravity);
        Physics.gravity = new Vector3(0, newGravity, 0);
        UpdateGravityDisplay();
    }

    private void UpdateGravityDisplay()
    {
        if (gravityDisplay != null)
        {
            // Calculate gravity in g's (relative to Earth's gravity)
            float currentGravityInGs = Physics.gravity.y / EarthGravity;
            gravityDisplay.text = $"{currentGravityInGs:F2} g"; // Display with two decimal points
        }
    }
}
