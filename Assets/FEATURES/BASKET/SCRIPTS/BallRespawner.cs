using UnityEngine;
using TMPro;

namespace starskyproductions.playground.ballrespawn
{
    /// <summary>
    /// Handles ball respawning triggered by a UI button and provides visual/auditory feedback.
    /// </summary>
    public class BallRespawner : MonoBehaviour
    {
        #region PUBLIC PROPERTIES
        [Header("Respawn Settings")]
        [Tooltip("TextMeshPro object to display feedback.")]
        [SerializeField] private TextMeshPro feedbackDisplay;

        [Tooltip("Time (in seconds) to display feedback messages.")]
        [SerializeField] private float feedbackDuration = 2.0f;

        [Tooltip("AudioSource to play respawn sound.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("Sound to play when the ball is respawned.")]
        [SerializeField] private AudioClip respawnSound;
        #endregion

        #region PRIVATE FIELDS
        private Vector3 initialPosition;
        private Rigidbody ballRigidbody;
        #endregion

        #region UNITY METHODS
        private void Start()
        {
            // Save the initial position of the ball
            initialPosition = transform.position;

            // Ensure the Rigidbody is assigned
            ballRigidbody = GetComponent<Rigidbody>();
            if (ballRigidbody == null)
            {
                Debug.LogError("Rigidbody is not assigned or found on the ball GameObject.");
            }

            // Ensure the AudioSource is assigned
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned or found on the ball GameObject.");
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Respawns the ball at its initial position.
        /// </summary>
        public void RespawnBall()
        {
            if (ballRigidbody != null)
            {
                // Reset position and velocity
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
                transform.position = initialPosition;

                // Play respawn sound
                PlayRespawnSound();

                // Show feedback message
                DisplayMessage("Ball Ready", Color.green);

                Debug.Log("Ball respawned to starting position.");
            }
            else
            {
                Debug.LogWarning("Rigidbody is missing; unable to respawn the ball.");
            }
        }
        #endregion

        #region PRIVATE METHODS
        private void PlayRespawnSound()
        {
            if (audioSource != null && respawnSound != null)
            {
                audioSource.PlayOneShot(respawnSound);
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
