using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard TMP Objects")]
    [Tooltip("TMP object for displaying ranks 1-4.")]
    [SerializeField] private TextMeshPro leaderboardTMPTop4;

    [Tooltip("TMP object for displaying ranks 5-8.")]
    [SerializeField] private TextMeshPro leaderboardTMPBottom4;

    [Header("Settings")]
    [Tooltip("Maximum number of scores to display.")]
    [SerializeField] private int maxEntries = 8;

    private List<int> scores = new List<int>();

    private void Awake()
    {
        if (leaderboardTMPTop4 == null || leaderboardTMPBottom4 == null)
        {
            Debug.LogError("[LeaderboardManager] TMP objects are not assigned in the Inspector!");
        }
    }

    public void AddScore(int newScore)
    {
        Debug.Log($"[LeaderboardManager] Adding new score: {newScore}");
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a)); // Sort scores in descending order

        if (scores.Count > maxEntries)
        {
            int removedScore = scores[scores.Count - 1];
            scores.RemoveAt(scores.Count - 1);
            Debug.Log($"[LeaderboardManager] Removed lowest score: {removedScore}");
        }

        UpdateLeaderboardDisplay();
    }

    private void UpdateLeaderboardDisplay()
    {
        Debug.Log("[LeaderboardManager] Updating leaderboard display...");

        // Update the top 4 TMP
        leaderboardTMPTop4.text = "";
        for (int i = 0; i < 4; i++)
        {
            if (i < scores.Count)
            {
                leaderboardTMPTop4.text += $"{i + 1}. {scores[i]:D3}\n"; // Add rank and score
            }
            else
            {
                leaderboardTMPTop4.text += $"{i + 1}. 000\n"; // Default display
            }
        }

        // Update the bottom 4 TMP
        leaderboardTMPBottom4.text = "";
        for (int i = 4; i < maxEntries; i++)
        {
            int index = i - 4;
            if (i < scores.Count)
            {
                leaderboardTMPBottom4.text += $"{i + 1}. {scores[i]:D3}\n"; // Add rank and score
            }
            else
            {
                leaderboardTMPBottom4.text += $"{i + 1}. 000\n"; // Default display
            }
        }
    }

    public void ClearLeaderboard()
    {
        scores.Clear();
        Debug.Log("[LeaderboardManager] Leaderboard cleared.");

        // Reset TMP objects
        leaderboardTMPTop4.text = "1. 000\n2. 000\n3. 000\n4. 000\n";
        leaderboardTMPBottom4.text = "5. 000\n6. 000\n7. 000\n8. 000\n";
    }
}
