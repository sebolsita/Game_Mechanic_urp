using UnityEngine;
using TMPro;
using System.Collections;
using starskyproductions.playground.scoring;

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
        [Tooltip("Game duration in seconds (1 minute = 60 seconds).")]
        [SerializeField] private int gameDuration = 60;

        [Header("UI Elements")]
        [Tooltip("TMP for displaying countdowns, streaks, and messages.")]
        [SerializeField] private TextMeshPro gameMessageTMP;
        [Tooltip("TMP for the timer.")]
        [SerializeField] private TextMeshPro timerTMP;

        [Header("Audio")]
        [Tooltip("Sound for the game start countdown.")]
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
        [Tooltip("Default message color.")]
        [SerializeField] private Color defaultMessageColor = new Color32(35, 178, 31, 255); // Hex: #23B21F
        [Tooltip("Color for 'GAME ABORTED' message.")]
        [SerializeField] private Color abortColor = Color.red;
        #endregion

        private GameState currentState = GameState.Paused;
        private int currentTime;
        private bool scoreEnabled = false;
        private bool isPaused = false; // Tracks whether the game is paused
        [Header("Basketball Settings")]
        [Tooltip("Assign the basketball Rigidbody here.")]
        [SerializeField] private Rigidbody basketballRigidbody;



        #region UNITY METHODS
        private void Start()
        {
            ResetGame();

            // Automatically find the basketball
            basketballRigidbody = FindObjectOfType<Rigidbody>();
            if (basketballRigidbody == null)
            {
                Debug.LogError("No basketball found in the scene! Please add a Rigidbody to the basketball.");
            }
        }

        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Starts the game with the proper message and countdown sequence.
        /// </summary>
        public void StartGame()
        {
            if (currentState == GameState.Running || isPaused)
            {
                Debug.Log("Cannot start a new game while running or paused.");
                return;
            }

            currentState = GameState.Paused;
            DisplayMessage("GET READY", defaultMessageColor);
            StartCoroutine(CountdownAndStart());
        }

        /// <summary>
        /// Pauses or unpauses the game.
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Running)
            {
                isPaused = true;
                currentState = GameState.Paused;
                DisplayMessage("Game Paused", defaultMessageColor);
                Debug.Log("Game Paused");

                // Freeze the basketball
                FreezeBasketball(true);
            }
            else if (currentState == GameState.Paused && isPaused)
            {
                isPaused = false;
                currentState = GameState.Running;
                StartCoroutine(DisplayTemporaryMessage("Game Resumed", defaultMessageColor, 2f));
                Debug.Log("Game Resumed");

                // Unfreeze the basketball
                FreezeBasketball(false);
            }
        }


        private void FreezeBasketball(bool freeze)
        {
            if (basketballRigidbody != null)
            {
                basketballRigidbody.isKinematic = freeze;
                if (freeze)
                {
                    basketballRigidbody.velocity = Vector3.zero;
                    basketballRigidbody.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("Basketball Rigidbody not found. Cannot freeze/unfreeze.");
            }
        }






        /// <summary>
        /// Aborts the current game.
        /// </summary>
        public void AbortGame()
        {
            StopAmbientMusic();
            currentState = GameState.Paused;
            isPaused = false;
            scoreEnabled = false;

            // Reset game state
            ResetGame();

            // Display "GAME ABORTED" message for 2 seconds
            StartCoroutine(DisplayTemporaryMessage("GAME ABORTED", abortColor, 2f));
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Resets the game to its initial state.
        /// </summary>
        private void ResetGame()
        {
            currentState = GameState.Paused;
            currentTime = gameDuration;
            scoreEnabled = false;

            UpdateTimerDisplay();
            timerTMP.color = defaultMessageColor; // Reset timer color
            DisplayMessage("TIMED GAME", defaultMessageColor);

            // Ensure basketball is unfrozen (non-kinematic)
            var basketball = FindObjectOfType<Rigidbody>();
            if (basketball != null)
            {
                basketball.isKinematic = true; // Make sure it's frozen at reset
            }

            // Notify the scoring system to reset the score
            var scoringSystem = FindObjectOfType<BasketballScoringSystem>();
            if (scoringSystem != null)
            {
                scoringSystem.ResetScore();
            }
        }
        /// <summary>
        /// Plays the countdown sequence and starts the game.
        /// </summary>
        private IEnumerator CountdownAndStart()
        {
            if (audioSource != null && countdownSound != null)
            {
                audioSource.PlayOneShot(countdownSound); // Play countdown audio
            }

            yield return new WaitForSeconds(2f); // Wait for "GET READY"
            DisplayMessage("3", dangerColor);
            yield return new WaitForSeconds(1f);

            DisplayMessage("2", warningColor);
            yield return new WaitForSeconds(1f);

            DisplayMessage("1", Color.green);
            yield return new WaitForSeconds(1f);

            StartGameplay();
        }

        /// <summary>
        /// Begins gameplay.
        /// </summary>
        private void StartGameplay()
        {
            currentState = GameState.Running;
            scoreEnabled = true; // Enable scoring
            DisplayMessage($"SCORE: 0", defaultMessageColor);
            StartCoroutine(GameTimer());

            // Start ambient sound
            StartAmbientMusic();
        }

        /// <summary>
        /// Returns whether scoring is currently enabled.
        /// </summary>
        public bool IsScoreEnabled()
        {
            return scoreEnabled;
        }

        /// <summary>
        /// Manages the game timer.
        /// </summary>
        private IEnumerator GameTimer()
        {
            while (currentTime > 0)
            {
                if (!isPaused) // Only decrement time when not paused
                {
                    yield return new WaitForSeconds(1f);
                    currentTime--;

                    // Update the timer display and color for the last seconds
                    if (currentTime <= 10)
                    {
                        timerTMP.color = (currentTime <= 3) ? dangerColor : warningColor;
                    }

                    UpdateTimerDisplay();
                }
                else
                {
                    yield return null; // Wait until unpaused
                }
            }

            if (currentTime <= 0 && currentState == GameState.Running)
            {
                EndGame();
            }
        }





        /// <summary>
        /// Ends the game and resets to the initial state.
        /// </summary>
        private void EndGame()
        {
            currentState = GameState.GameOver;
            scoreEnabled = false; // Disable scoring
            DisplayMessage("GAME OVER", dangerColor);

            StopAmbientMusic();
            PlaySound(endSound);

            StartCoroutine(ShowGameOverAndReset());
        }

        /// <summary>
        /// Displays "GAME OVER" for 2 seconds, then resets to "TIMED GAME."
        /// </summary>
        private IEnumerator ShowGameOverAndReset()
        {
            yield return new WaitForSeconds(2f);
            ResetGame();
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
        /// Displays a message with a specific color.
        /// </summary>
        private void DisplayMessage(string message, Color color)
        {
            if (gameMessageTMP != null)
            {
                gameMessageTMP.text = message;
                gameMessageTMP.color = color;
            }
        }

        /// <summary>
        /// Updates the score message dynamically.
        /// </summary>
        public void UpdateScoreDisplay(int score)
        {
            if (currentState == GameState.Running)
            {
                DisplayMessage($"SCORE: {score}", defaultMessageColor);
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
        /// Starts the ambient music.
        /// </summary>
        private void StartAmbientMusic()
        {
            if (audioSource != null && ambientSound != null)
            {
                audioSource.clip = ambientSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Stops the ambient music.
        /// </summary>
        private void StopAmbientMusic()
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Pauses the ambient music.
        /// </summary>
        private void PauseAmbientMusic()
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

        /// <summary>
        /// Resumes the ambient music.
        /// </summary>
        private void ResumeAmbientMusic()
        {
            if (audioSource != null)
            {
                audioSource.UnPause();
            }
        }

        /// <summary>
        /// Displays the "GAME ABORTED" message for 2 seconds, then resets the game.
        /// </summary>
        private IEnumerator DisplayAbortMessage()
        {
            DisplayMessage("GAME ABORTED", abortColor);
            yield return new WaitForSeconds(2f);
            ResetGame();
        }

        /// <summary>
        /// Displays a temporary message for a specified duration, then reverts to the timer or default message.
        /// </summary>
        private IEnumerator DisplayTemporaryMessage(string message, Color color, float duration)
        {
            if (gameMessageTMP != null)
            {
                gameMessageTMP.text = message;
                gameMessageTMP.color = color;
            }

            yield return new WaitForSeconds(duration);

            // Revert to showing the timer or default message
            if (currentState == GameState.Running)
            {
                DisplayMessage($"SCORE: {FindObjectOfType<BasketballScoringSystem>()?  .GetCurrentScore() ?? 0}", defaultMessageColor);
            }
            else
            {
                DisplayMessage("TIMED GAME", defaultMessageColor);
            }
        }
        #endregion
    }
}
