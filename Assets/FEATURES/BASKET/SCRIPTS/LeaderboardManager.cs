using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard TMP Objects")]
    [SerializeField] private TextMeshPro leaderboardTMPTop4;
    [SerializeField] private TextMeshPro leaderboardTMPBottom4;

    [Header("Settings")]
    [SerializeField] private int maxEntries = 8;

    private List<int> scores = new List<int>();

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

        UpdateLeaderboardDisplay(); // Ensure TMP objects are updated
    }

    private void UpdateLeaderboardDisplay()
    {
        Debug.Log("[LeaderboardManager] Updating leaderboard display...");

        leaderboardTMPTop4.text = ""; // Reset top 4 text
        leaderboardTMPBottom4.text = ""; // Reset bottom 4 text

        for (int i = 0; i < scores.Count; i++)
        {
            string entry = $"{i + 1}. {scores[i]:D3}"; // Rank and score
            if (i < 4)
            {
                leaderboardTMPTop4.text += entry + "\n"; // Add to top 4 TMP
                Debug.Log($"[LeaderboardManager] Top 4 Entry: {entry}");
            }
            else
            {
                leaderboardTMPBottom4.text += entry + "\n"; // Add to bottom 4 TMP
                Debug.Log($"[LeaderboardManager] Bottom 4 Entry: {entry}");
            }
        }

        // Fill remaining slots with default values if there are fewer than 8 scores
        for (int i = scores.Count; i < maxEntries; i++)
        {
            string defaultEntry = $"{i + 1}. 000";
            if (i < 4)
            {
                leaderboardTMPTop4.text += defaultEntry + "\n";
            }
            else
            {
                leaderboardTMPBottom4.text += defaultEntry + "\n";
            }
        }
    }

    public void ClearLeaderboard()
    {
        scores.Clear();
        Debug.Log("[LeaderboardManager] Leaderboard cleared.");
        UpdateLeaderboardDisplay();
    }
}
