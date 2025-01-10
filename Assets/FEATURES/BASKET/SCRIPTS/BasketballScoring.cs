using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace starskyproductions.playground
{
    public class BasketballScoringSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText; // Score display TMP object
        public UnityEvent<int> OnScoreUpdated;

        private int _currentScore;
        private int _lastZoneEntered = 3; // Default to Zone 3 (largest zone)

        private bool _scoreExited; // Tracks if ball exited "score_collider"

        private void Awake()
        {
            ZoneDetector.OnPlayerEnterZone += HandlePlayerEnterZone; // Subscribe to zone entry events
            HoopColliderDetector.OnBallHoopEvent += HandleBallHoopEvent;

            OnScoreUpdated ??= new UnityEvent<int>();
            OnScoreUpdated.AddListener(UpdateScoreDisplay);
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
            Debug.Log($"Awarding points based on last entered zone: Zone {_lastZoneEntered}");
            AddScore(_lastZoneEntered);
        }

        public void AddScore(int points)
        {
            _currentScore += points;
            Debug.Log($"Points added: {points}, Total score: {_currentScore}");
            OnScoreUpdated.Invoke(_currentScore);
        }

        private void UpdateScoreDisplay(int newScore)
        {
            if (_scoreText != null) _scoreText.text = $"Score: {newScore}";
        }
    }
}
