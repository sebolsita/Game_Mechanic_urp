using UnityEngine;
using TMPro;
using System.Collections;

namespace starskyproductions.playground
{
    /// <summary>
    /// Manages the core game logic, including countdowns, game states, and timer functionality.
    /// </summary>
    public class GameLogicManager : MonoBehaviour
    {
        public enum GameState { Paused, Running, GameOver }

        #region PUBLIC PROPERTIES
        [Header("Game Settings")]
        [Tooltip("Game duration in seconds (3 minutes = 180 seconds).")]
        [SerializeField] private int gameDuration = 180;

        [Header("UI Elements")]
        [Tooltip("TMP for displaying countdowns, streaks, and messages.")]
        [SerializeField] private TextMeshPro gameMessageTMP;
        [Tooltip("TMP for the timer.")]
        [SerializeField] private TextMeshPro timerTMP;

        [Header("Audio")]
        [Tooltip("Sound for each countdown number.")]
        [SerializeField] private AudioClip countdownSound;
        [Tooltip("Sound for the game start.")]
        [SerializeField] private AudioClip startSound;
        [Tooltip("Sound to play at the end of the game.")]
        [SerializeField] private AudioClip endSound;
        [Tooltip("Ambient background sound.")]
        [SerializeField] private AudioClip ambientSound;
        [Tooltip("AudioSource for playing sounds.")]
        [SerializeField] private AudioSource audioSource;

        [Header("Colors")]
        [Tooltip("Timer color for the last 10 seconds.")]
        [SerializeField] private Color warningColor = Color.yellow;
        [Tooltip("Timer color for the last 3 seconds.")]
        [SerializeField] private Color dangerColor = Color.red;
        #endregion

        private GameState currentState = GameState.Paused;
        private int currentTime;

        #region UNITY METHODS
        private void Start()
        {
            ResetGame();
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Starts the game with a countdown.
        /// </summary>
        public void StartGame()
        {
            currentState = GameState.Paused;
            StartAmbientSound();
            StartCoroutine(CountdownAndStart());
        }

        /// <summary>
        /// Pauses or unpauses the game.
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Running)
            {
                currentState = GameState.Paused;
                DisplayMessage("Game Paused");
            }
            else if (currentState == GameState.Paused)
            {
                currentState = GameState.Running;
                DisplayMessage("Game Resumed");
            }
        }

        /// <summary>
        /// Aborts the current game.
        /// </summary>
        public void AbortGame()
        {
            ResetGame();
            DisplayMessage("Game Aborted");
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Resets the game state.
        /// </summary>
        private void ResetGame()
        {
            currentState = GameState.Paused;
            currentTime = gameDuration;
            UpdateTimerDisplay();
            DisplayMessage("Ready");
        }

        /// <summary>
        /// Handles the countdown and starts the game.
        /// </summary>
        private IEnumerator CountdownAndStart()
        {
            for (int i = 5; i > 0; i--)
            {
                DisplayMessage(i.ToString());
                PlaySound(countdownSound);
                yield return new WaitForSeconds(1f);
            }

            DisplayMessage("START");
            PlaySound(startSound);
            yield return new WaitForSeconds(1f);

            currentState = GameState.Running;
            StartCoroutine(GameTimer());
        }

        /// <summary>
        /// Plays the ambient sound.
        /// </summary>
        private void StartAmbientSound()
        {
            if (audioSource != null && ambientSound != null)
            {
                audioSource.clip = ambientSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Manages the game timer.
        /// </summary>
        private IEnumerator GameTimer()
        {
            while (currentTime > 0 && currentState == GameState.Running)
            {
                yield return new WaitForSeconds(1f);
                currentTime--;

                // Flash timer color for the last 10 seconds
                if (currentTime <= 10)
                {
                    timerTMP.color = (currentTime <= 3) ? dangerColor : warningColor;
                }

                UpdateTimerDisplay();
            }

            if (currentTime == 0)
            {
                EndGame();
            }
        }

        /// <summary>
        /// Ends the game and plays the end sound.
        /// </summary>
        private void EndGame()
        {
            currentState = GameState.GameOver;
            DisplayMessage("Game Over");

            // Stop ambient sound
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            // Play end sound
            PlaySound(endSound);

            SaveScore();
        }

        /// <summary>
        /// Updates the timer TMP.
        /// </summary>
        private void UpdateTimerDisplay()
        {
            int minutes = currentTime / 60;
            int seconds = currentTime % 60;
            timerTMP.text = $"{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// Displays a message on the shared TMP.
        /// </summary>
        private void DisplayMessage(string message)
        {
            if (gameMessageTMP != null)
            {
                gameMessageTMP.text = message;
            }
        }

        /// <summary>
        /// Plays a sound using the AudioSource.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Saves the current score (placeholder for leaderboard integration).
        /// </summary>
        private void SaveScore()
        {
            // Placeholder for leaderboard logic
            Debug.Log($"Score saved. Timer reached zero.");
        }
        #endregion
    }
}