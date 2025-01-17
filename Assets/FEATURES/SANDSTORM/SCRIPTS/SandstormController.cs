using System.Collections;
using UnityEngine;
using TMPro;

public class SandstormController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Audio sources to toggle on/off.")]
    [SerializeField] private AudioSource[] audioSources;
    [Tooltip("Duration of the volume fade-in effect in seconds.")]
    [SerializeField] private float fadeDuration = 2.0f;

    [Header("Particle Systems")]
    [Tooltip("Particle systems to toggle on/off.")]
    [SerializeField] private ParticleSystem[] particleSystems;

    [Header("Sandstorm Settings")]
    [Tooltip("Minimum duration of a sandstorm (in seconds).")]
    [SerializeField] private float sandstormMinDuration = 120f; // 2 minutes
    [Tooltip("Maximum duration of a sandstorm (in seconds).")]
    [SerializeField] private float sandstormMaxDuration = 360f; // 6 minutes
    [Tooltip("Minimum interval between sandstorms (in seconds).")]
    [SerializeField] private float intervalMinDuration = 120f; // 2 minutes
    [Tooltip("Maximum interval between sandstorms (in seconds).")]
    [SerializeField] private float intervalMaxDuration = 720f; // 12 minutes

    [Header("UI Settings")]
    [Tooltip("TextMeshPro object to display the current state.")]
    [SerializeField] private TMP_Text stateDisplay;

    private enum SandstormState { On, Off, Random }
    private SandstormState currentState = SandstormState.On;

    private void Start()
    {
        UpdateStateDisplay();

        if (currentState == SandstormState.On)
        {
            StartCoroutine(InitializeSystemsSmoothly());
        }
        else if (currentState == SandstormState.Random)
        {
            StartCoroutine(SandstormRoutine());
        }
    }

    /// <summary>
    /// Toggles the sandstorm state between On, Off, and Random.
    /// </summary>
    public void ToggleSandstormState()
    {
        // Cycle through states: On -> Off -> Random -> On
        currentState = (SandstormState)(((int)currentState + 1) % 3);
        UpdateStateDisplay();

        StopAllCoroutines();

        if (currentState == SandstormState.On)
        {
            ToggleSystems(true);
        }
        else if (currentState == SandstormState.Off)
        {
            ToggleSystems(false);
        }
        else if (currentState == SandstormState.Random)
        {
            StartCoroutine(SandstormRoutine());
        }
    }

    /// <summary>
    /// Updates the TextMeshPro display to reflect the current state.
    /// </summary>
    private void UpdateStateDisplay()
    {
        if (stateDisplay != null)
        {
            stateDisplay.text = $"{currentState}";
        }
    }

    /// <summary>
    /// Toggles audio sources and particle systems on/off.
    /// </summary>
    private void ToggleSystems(bool state)
    {
        // Toggle audio sources
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                if (state)
                {
                    StartCoroutine(FadeInAudio(audioSource));
                }
                else
                {
                    StartCoroutine(FadeOutAudio(audioSource));
                }
            }
        }

        // Toggle particle systems
        foreach (var particleSystem in particleSystems)
        {
            if (particleSystem != null)
            {
                if (state)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
        }
    }

    /// <summary>
    /// Routine to manage the timing of sandstorms in Random mode.
    /// </summary>
    private IEnumerator SandstormRoutine()
    {
        while (currentState == SandstormState.Random)
        {
            // Wait for a random interval before starting the next sandstorm
            float intervalDuration = Random.Range(intervalMinDuration, intervalMaxDuration);
            Debug.Log($"Next sandstorm in {intervalDuration / 60f:F2} minutes.");
            yield return new WaitForSeconds(intervalDuration);

            // Determine sandstorm duration
            float availableTime = intervalMaxDuration - intervalDuration;
            float sandstormDuration = Random.Range(sandstormMinDuration, Mathf.Min(sandstormMaxDuration, availableTime));
            Debug.Log($"Sandstorm starting for {sandstormDuration / 60f:F2} minutes.");

            // Activate sandstorm
            ToggleSystems(true);

            // Wait for the sandstorm to finish
            yield return new WaitForSeconds(sandstormDuration);

            // Deactivate sandstorm
            ToggleSystems(false);
        }
    }

    /// <summary>
    /// Smoothly initializes the systems at startup.
    /// </summary>
    private IEnumerator InitializeSystemsSmoothly()
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                StartCoroutine(FadeInAudio(audioSource));
            }
        }

        foreach (var particleSystem in particleSystems)
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        yield return null; // Ensure systems start smoothly
    }

    /// <summary>
    /// Gradually fades in the volume of an audio source.
    /// </summary>
    private IEnumerator FadeInAudio(AudioSource audioSource)
    {
        float targetVolume = audioSource.volume > 0 ? audioSource.volume : 1f; // Default to 1 if not set
        audioSource.volume = 0;
        audioSource.Play();

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume; // Ensure final volume is set
    }

    /// <summary>
    /// Gradually fades out the volume of an audio source.
    /// </summary>
    private IEnumerator FadeOutAudio(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0; // Ensure final volume is set to 0
        audioSource.Stop();
    }
}