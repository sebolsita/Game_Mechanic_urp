using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace starskyproductions.playground.scoring
{
    public class BasketballScoringSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _scoreText; // Score display TMP object
        [SerializeField] private GameLogicManager gameLogicManager; // Reference to GameLogicManager

        public UnityEvent<int> OnScoreUpdated;

        private int _currentScore;
        private int _lastZoneEntered = 3; // Default to Zone 3 (largest zone)

        private bool _scoreExited; // Tracks if ball exited "score_collider"

        // Reference to the GameLogicManager
        private GameLogicManager _gameLogicManager;

        private void Awake()
        {
            // Find the GameLogicManager in the scene
            _gameLogicManager = FindObjectOfType<GameLogicManager>();
            if (_gameLogicManager == null)
            {
                Debug.LogError("GameLogicManager not found in the scene. Scoring will be disabled.");
            }

            ZoneDetector.OnPlayerEnterZone += HandlePlayerEnterZone; // Subscribe to zone entry events
            HoopColliderDetector.OnBallHoopEvent += HandleBallHoopEvent;

            OnScoreUpdated ??= new UnityEvent<int>();
            OnScoreUpdated.AddListener(UpdateScoreDisplay);

            gameLogicManager = FindObjectOfType<GameLogicManager>();
            if (gameLogicManager == null)
            {
                Debug.LogError("GameLogicManager not found in the scene.");
            }
        }

        private void OnDestroy()
        {
            ZoneDetector.OnPlayerEnterZone -= HandlePlayerEnterZone; // Unsubscribe from zone events
            HoopColliderDetector.OnBallHoopEvent -= HandleBallHoopEvent;
        }

        private void HandlePlayerEnterZone(int zoneScore)
        {
            _lastZoneEntered = zoneScore; // Update the last entered zone
            Debug.Log($"Last zone entered updated to: Zone {_lastZoneEntered}");
        }

        private void HandleBallHoopEvent(string colliderName, bool isExiting)
        {
            if (colliderName == "score_collider" && !isExiting)
            {
                Debug.Log("Ball entered score_collider.");
                _scoreExited = false; // Reset for a new sequence
            }
            else if (colliderName == "score_collider" && isExiting)
            {
                Debug.Log("Ball exited score_collider.");
                _scoreExited = true; // Mark that the ball has exited score_collider
            }
            else if (colliderName == "cheat_collider" && !isExiting && _scoreExited)
            {
                Debug.Log("Ball entered cheat_collider after exiting score_collider. Valid scoring sequence.");
            }
            else if (colliderName == "cheat_collider" && isExiting && _scoreExited)
            {
                Debug.Log("Ball exited cheat_collider. Determining score.");
                AwardPoints();
                _scoreExited = false; // Reset sequence
            }
            else
            {
                Debug.Log($"Invalid hoop sequence. Ball entered/exited: {colliderName}, Is Exiting: {isExiting}");
            }
        }

        private void AwardPoints()
        {
            // Check if scoring is enabled in the GameLogicManager
            if (_gameLogicManager != null && !_gameLogicManager.IsScoreEnabled())
            {
                Debug.Log("Scoring is currently disabled. No points awarded.");
                return;
            }

            Debug.Log($"Awarding points based on last entered zone: Zone {_lastZoneEntered}");
            AddScore(_lastZoneEntered);
        }

        public void AddScore(int points)
        {
            _currentScore += points;
            Debug.Log($"Points added: {points}, Total score: {_currentScore}");
            OnScoreUpdated.Invoke(_currentScore);

            // Notify the GameLogicManager about the score update
            if (gameLogicManager != null)
            {
                gameLogicManager.UpdateScoreDisplay(_currentScore);
            }
        }

        private void UpdateScoreDisplay(int newScore)
        {
            if (_scoreText != null) _scoreText.text = $"{newScore}";
        }

        public void ResetScore()
        {
            _currentScore = 0;
            Debug.Log("Score has been reset.");
            OnScoreUpdated.Invoke(_currentScore);

            // Display the reset score immediately
            if (_scoreText != null)
            {
                _scoreText.text = "0";
            }

            // Vibrate both controllers (Oculus example)
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch); // Left controller
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch); // Right controller

            Invoke(nameof(StopHaptics), 0.1f); // Stop vibration after 0.1 seconds
        }

        /// <summary>
        /// Returns the current score.
        /// </summary>
        public int GetCurrentScore()
        {
            return _currentScore;
        }


        private void StopHaptics()
        {
            // Stop vibration on both controllers
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
