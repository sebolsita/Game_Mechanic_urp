using UnityEngine;
using TMPro;

public class GameModeSwitcher : MonoBehaviour
{
    public enum GameMode
    {
        Freestyle,
        Timed
    }

    [Header("UI Feedback")]
    [Tooltip("TextMeshPro object to display the current game mode.")]
    [SerializeField] private TextMeshProUGUI modeDisplay;

    [Header("Initial Settings")]
    [Tooltip("The starting game mode.")]
    [SerializeField] private GameMode currentGameMode = GameMode.Freestyle;

    private void Start()
    {
        UpdateModeDisplay(); // Initialize display with the starting game mode
    }

    /// <summary>
    /// Toggles the game mode between Freestyle and Timed.
    /// </summary>
    public void ToggleGameMode()
    {
        currentGameMode = (currentGameMode == GameMode.Freestyle) ? GameMode.Timed : GameMode.Freestyle;
        UpdateModeDisplay();
    }

    /// <summary>
    /// Updates the TextMeshPro object with the current game mode.
    /// </summary>
    private void UpdateModeDisplay()
    {
        if (modeDisplay != null)
        {
            modeDisplay.text = $"{currentGameMode}";
        }
    }

    /// <summary>
    /// Gets the current game mode.
    /// </summary>
    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }
}
