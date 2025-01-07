using System.Collections;
using UnityEngine;

namespace starskyproductions.playground.audio
{
    public class BasketballAudio : MonoBehaviour
    {

        #region PUBLIC PROPERTIES
        [Tooltip("The minimum pitch for low collision strength.")]
        public float MinPitch = 0.8f;

        [Tooltip("The maximum pitch for high collision strength.")]
        public float MaxPitch = 1.5f;

        [Tooltip("The maximum velocity that maps to MaxPitch.")]
        public float MaxCollisionVelocity = 10f;

        [Tooltip("The velocity threshold for playing a fast catch sound.")]
        public float CatchVelocityThreshold = 2.0f;
        #endregion

        [Tooltip("Audio source for playing collision sounds.")]
        [SerializeField] private AudioSource _audioSource;

        [Tooltip("Audio clips to play on collision.")]
        [SerializeField] private AudioClip[] _collisionClips;

        [Tooltip("Audio clips to play when the ball hits the net.")]
        [SerializeField] private AudioClip[] _netCollisionClips;

        [Tooltip("Audio clips to play when the ball is caught.")]
        [SerializeField] private AudioClip _slowCatchClip;
        [SerializeField] private AudioClip _fastCatchClip;

        #region UNITY METHODS

        private void Awake()
        {
            if (_audioSource == null)
                Debug.LogError("AudioSource is not assigned. Assign it in the inspector.");

            if (_collisionClips == null || _collisionClips.Length == 0)
                Debug.LogError("Collision AudioClips are not assigned. Assign them in the inspector.");

            if (_netCollisionClips == null || _netCollisionClips.Length == 0)
                Debug.LogError("Net Collision AudioClips are not assigned. Assign them in the inspector.");

            if (_slowCatchClip == null || _fastCatchClip == null)
                Debug.LogError("Catch AudioClips are not assigned. Assign them in the inspector.");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Net"))
            {
                PlayNetCollisionSound();
            }
            else
            {
                PlayRandomCollisionSound(collision);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Net"))
            {
                PlayNetCollisionSound();
            }
        }

        public void OnCatchBall(Vector3 ballVelocity)
        {
            PlayCatchSound(ballVelocity.magnitude);
        }

        #endregion

        #region PRIVATE METHODS

        private void PlayRandomCollisionSound(Collision collision)
        {
            if (_audioSource == null || _collisionClips == null || _collisionClips.Length == 0) return;

            // Select a random clip from the array.
            AudioClip selectedClip = GetRandomClip(_collisionClips);

            // Calculate collision strength based on relative velocity.
            float collisionStrength = collision.relativeVelocity.magnitude;

            // Map the collision strength to the pitch range.
            float normalizedStrength = Mathf.Clamp01(collisionStrength / MaxCollisionVelocity);
            float pitch = Mathf.Lerp(MinPitch, MaxPitch, normalizedStrength);

            // Set the pitch and play the selected sound.
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(selectedClip);
        }

        private void PlayNetCollisionSound()
        {
            if (_audioSource == null || _netCollisionClips == null || _netCollisionClips.Length == 0) return;

            // Select a random clip from the net collision pool.
            AudioClip selectedClip = GetRandomClip(_netCollisionClips);

            // Set the clip and adjust volume.
            _audioSource.clip = selectedClip;
            _audioSource.volume *= 0.5f; // Reduce volume by 33%

            // Play the net collision sound with default pitch.
            _audioSource.pitch = 1.0f;
            _audioSource.Play();

            // Restore original volume after the clip duration.
            StartCoroutine(RestoreVolumeAfterClip(selectedClip.length));
        }

        private IEnumerator RestoreVolumeAfterClip(float clipLength)
        {
            yield return new WaitForSeconds(clipLength);
            _audioSource.volume /= 0.5f; // Restore the original volume
        }

        private void PlayCatchSound(float velocity)
        {
            if (_audioSource == null || _slowCatchClip == null || _fastCatchClip == null) return;

            // Determine which catch sound to play based on velocity.
            AudioClip selectedClip = velocity > CatchVelocityThreshold ? _fastCatchClip : _slowCatchClip;

            // Play the catch sound.
            _audioSource.pitch = 1.0f;
            _audioSource.PlayOneShot(selectedClip);
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            int randomIndex = Random.Range(0, clips.Length);
            return clips[randomIndex];
        }

        #endregion

    }
}
