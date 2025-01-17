using UnityEngine;
using TMPro;
using Meta.XR.MRUtilityKit;

public class ARBasketballManager : MonoBehaviour
{
    #region PUBLIC PROPERTIES
    public GameObject basketballGamePrefab; // Parent prefab containing hoop, court, and UI
    public GameObject passthroughBuildingBlock; // GameObject to enable AR camera view
    public TextMeshProUGUI stateText; // Text to display AR mode state
    #endregion

    #region PRIVATE VARIABLES
    private GameObject _basketballGameInstance;
    private bool _isARMode = false; // Tracks AR mode state
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        UpdateStateText();
    }

    public void ToggleARMode()
    {
        _isARMode = !_isARMode;

        if (_isARMode)
        {
            EnableARMode();
        }
        else
        {
            DisableARMode();
        }

        UpdateStateText();
    }
    #endregion

    #region PRIVATE METHODS
    private void UpdateStateText()
    {
        stateText.text = _isARMode ? "AR Mode: ON" : "AR Mode: OFF";
    }

    private void EnableARMode()
    {
        Debug.Log("Enabling AR mode...");

        // Activate passthrough to enable camera view
        if (passthroughBuildingBlock != null)
        {
            passthroughBuildingBlock.SetActive(true);
            Debug.Log("Passthrough building block activated.");
        }
        else
        {
            Debug.LogError("Passthrough building block is not assigned in the inspector.");
        }

        // Find the key wall and place basketball game prefab
        DetectKeyWallAndPlaceObjects();
    }

    private void DisableARMode()
    {
        Debug.Log("Disabling AR mode...");

        // Deactivate passthrough
        if (passthroughBuildingBlock != null)
        {
            passthroughBuildingBlock.SetActive(false);
            Debug.Log("Passthrough building block deactivated.");
        }

        // Destroy dynamically placed basketball game prefab
        if (_basketballGameInstance != null)
        {
            Destroy(_basketballGameInstance);
            Debug.Log("Basketball game prefab removed.");
        }
    }

    private void DetectKeyWallAndPlaceObjects()
    {
        MRUKAnchor keyWall = MRUK.Instance?.GetCurrentRoom()?.GetKeyWall(out Vector2 wallScale);
        if (keyWall != null)
        {
            PlaceBasketballGameOnKeyWall(keyWall);
        }
        else
        {
            Debug.LogError("No key wall detected in the current room.");
        }
    }

    private void PlaceBasketballGameOnKeyWall(MRUKAnchor keyWall)
    {
        // Align basketball game prefab to the key wall
        Vector3 wallPosition = keyWall.transform.position;
        Quaternion wallRotation = Quaternion.LookRotation(-keyWall.transform.forward, Vector3.up);

        _basketballGameInstance = Instantiate(basketballGamePrefab, wallPosition, wallRotation);
        Debug.Log($"Basketball game prefab placed at {wallPosition} with rotation {wallRotation}");
    }
    #endregion
}
