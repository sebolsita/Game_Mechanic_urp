using UnityEngine;
using UnityEngine.Events;

public class VRButtonInteraction : MonoBehaviour
{
    [SerializeField] private UnityEvent OnButtonPressed; // Event triggered when the button is pressed

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand"))
        {
            Debug.Log("Button pressed with Left Hand.");
            TriggerHaptics(true); // True indicates left hand
            OnButtonPressed.Invoke(); // Trigger the assigned UnityEvent
        }
        else if (other.CompareTag("RightHand"))
        {
            Debug.Log("Button pressed with Right Hand.");
            TriggerHaptics(false); // False indicates right hand
            OnButtonPressed.Invoke(); // Trigger the assigned UnityEvent
        }
    }

    private void TriggerHaptics(bool isLeftHand)
    {
        if (isLeftHand)
        {
            // Trigger vibration for the left controller
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
            Invoke(nameof(StopLeftHaptics), 0.1f); // Stop after 0.1 seconds
        }
        else
        {
            // Trigger vibration for the right controller
            OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            Invoke(nameof(StopRightHaptics), 0.1f); // Stop after 0.1 seconds
        }
    }

    private void StopLeftHaptics()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    private void StopRightHaptics()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}
