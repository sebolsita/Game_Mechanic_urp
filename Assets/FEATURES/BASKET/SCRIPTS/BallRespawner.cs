using UnityEngine;
using TMPro;

namespace starskyproductions.playground.ballrespawn
{
    /// <summary>
    /// Handles ball respawn, inactivity management, and feedback messages.
    /// </summary>
    public class BallRespawner : MonoBehaviour
    {
        #region PUBLIC PROPERTIES
        [Header("Respawn Settings")]
        [Tooltip("Time in seconds before the ball is respawned if inactive.")]
        [SerializeField] private float inactivityTime = 3f;

        [Tooltip("TextMeshPro object to display feedback.")]
        [SerializeField] private TextMeshPro feedbackDisplay;

        [Tooltip("Time (in seconds) to display feedback messages.")]
        [SerializeField] private float feedbackDuration = 2.0f;

        [Tooltip("AudioSource to play respawn sound.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("Sound to play when the ball is respawned.")]
        [SerializeField] private AudioClip respawnSound;

        [Tooltip("Sound to play when respawn is blocked (e.g., during pause).")]
        [SerializeField] private AudioClip blockedSound;

        #endregion

        #region PRIVATE FIELDS
        private Vector3 initialPosition;
        private Rigidbody ballRigidbody;
        private float inactivityTimer = 0f;
        private bool isPaused = false;
        #endregion

        #region UNITY METHODS
        private void Start()
        {
            // Save the starting position of the ball
            initialPosition = transform.position;

            // Ensure Rigidbody and AudioSource are assigned
            ballRigidbody = GetComponent<Rigidbody>();
            if (ballRigidbody == null)
            {
                Debug.LogError("Rigidbody is not assigned or found on the ball GameObject.");
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned or found on the ball GameObject.");
            }
        }

        private void Update()
        {
            if (isPaused) return;

            // Check ball inactivity
            if (ballRigidbody.velocity.magnitude < 0.1f)
            {
                inactivityTimer += Time.deltaTime;
                if (inactivityTimer >= inactivityTime)
                {
                    RespawnBall();
                }
            }
            else
            {
                inactivityTimer = 0f; // Reset timer if the ball is moving
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Respawns the ball at its initial position.
        /// </summary>
        public void RespawnBall()
        {
            if (isPaused)
            {
                DisplayMessage("Game Paused", Color.red);
                PlaySound(blockedSound);
                return;
            }

            if (ballRigidbody != null)
            {
                // Reset position and velocity
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
                transform.position = initialPosition;

                // Play respawn sound
                PlaySound(respawnSound);

                // Show feedback message
                DisplayMessage("Ball Ready", Color.green);

                Debug.Log("Ball respawned to starting position.");
            }
            else
            {
                Debug.LogWarning("Ball Rigidbody is missing; unable to respawn.");
            }
        }

        /// <summary>
        /// Pauses or unpauses the ball's behavior.
        /// </summary>
        /// <param name="paused">True to pause, false to unpause.</param>
        public void SetPaused(bool paused)
        {
            isPaused = paused;

            if (ballRigidbody != null)
            {
                ballRigidbody.isKinematic = paused;
            }
        }
        #endregion

        #region PRIVATE METHODS
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        private void DisplayMessage(string message, Color color)
        {
            if (feedbackDisplay != null)
            {
                feedbackDisplay.text = message;
                feedbackDisplay.color = color;
                Invoke(nameof(ClearFeedback), feedbackDuration);
            }
        }

        private void ClearFeedback()
        {
            if (feedbackDisplay != null)
            {
                feedbackDisplay.text = "";
            }
        }
        #endregion
    }
}
