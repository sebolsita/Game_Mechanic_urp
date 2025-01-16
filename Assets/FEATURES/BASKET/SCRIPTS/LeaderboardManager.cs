using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    [Tooltip("TextMeshPro object for displaying the leaderboard.")]
    [SerializeField] private TextMeshProUGUI leaderboardTMP;

    [Tooltip("Maximum number of scores to display.")]
    [SerializeField] private int maxEntries = 8;

    private List<int> scores = new List<int>();

    /// <summary>
    /// Adds a new score to the leaderboard and updates the display.
    /// </summary>
    public void AddScore(int newScore)
    {
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a)); // Sort from highest to lowest

        if (scores.Count > maxEntries)
        {
            scores.RemoveAt(scores.Count - 1); // Remove the lowest score if over capacity
        }

        UpdateLeaderboardDisplay();
    }

    /// <summary>
    /// Updates the leaderboard TMP with the current scores.
    /// </summary>
    private void UpdateLeaderboardDisplay()
    {
        if (leaderboardTMP == null) return;

        leaderboardTMP.text = ""; // Clear existing text
        for (int i = 0; i < scores.Count; i++)
        {
            leaderboardTMP.text += $"{i + 1}. {scores[i]:D3}\n";
        }
    }
}
