using UnityEngine;
using TMPro;

namespace starskyproductions.playground.stat
{
    /// <summary>
    /// Tracks and displays game statistics, including streaks, total shots, and average time between shots.
    /// </summary>
    public class StatisticsManager : MonoBehaviour
    {
        #region PUBLIC PROPERTIES
        [Header("Statistics Settings")]
        [Tooltip("TMP object for displaying current streak.")]
        [SerializeField] private TextMeshProUGUI streakTMP;

        [Tooltip("TMP object for displaying highest streak.")]
        [SerializeField] private TextMeshProUGUI highestStreakTMP;

        [Tooltip("TMP object for displaying total shots.")]
        [SerializeField] private TextMeshProUGUI totalShotsTMP;

        [Tooltip("TMP object for displaying average time between shots.")]
        [SerializeField] private TextMeshProUGUI avgTimeTMP;
        #endregion

        #region PRIVATE FIELDS
        private int currentStreak = 0;
        private int highestStreak = 0;
        private int totalShots = 0;
        private float totalShotTime = 0f; // Cumulative time for all shots
        private float lastShotTime = 0f; // Time of the previous shot
        #endregion

        #region UNITY METHODS
        private void Start()
        {
            ResetStatistics();
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Resets all statistics for a new game.
        /// </summary>
        public void ResetStatistics()
        {
            currentStreak = 0;
            highestStreak = 0;
            totalShots = 0;
            totalShotTime = 0f;
            lastShotTime = 0f;
            UpdateDisplays();
        }

        /// <summary>
        /// Updates the streak and checks for a new high streak.
        /// </summary>
        /// <param name="streak">The new streak value.</param>
        public void UpdateStreak(int streak)
        {
            currentStreak = streak;
            if (currentStreak > highestStreak)
            {
                highestStreak = currentStreak;
            }
            UpdateDisplays();
        }

        /// <summary>
        /// Increments the total shots count and calculates the average time between shots.
        /// </summary>
        public void IncrementShots()
        {
            totalShots++;

            float currentTime = Time.time;
            if (lastShotTime > 0)
            {
                float timeSinceLastShot = currentTime - lastShotTime;
                totalShotTime += timeSinceLastShot;
            }
            lastShotTime = currentTime;

            UpdateDisplays();
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Updates all TMP displays with the latest statistics.
        /// </summary>
        private void UpdateDisplays()
        {
            if (streakTMP != null) streakTMP.text = $"{currentStreak}";
            if (highestStreakTMP != null) highestStreakTMP.text = $"{highestStreak}";
            if (totalShotsTMP != null) totalShotsTMP.text = $"{totalShots}";
            if (avgTimeTMP != null)
            {
                float avgTime = totalShots > 1 ? totalShotTime / (totalShots - 1) : 0f; // Avoid divide by zero
                avgTimeTMP.text = $"{avgTime:F2}s";
            }
        }
        #endregion
    }
}
