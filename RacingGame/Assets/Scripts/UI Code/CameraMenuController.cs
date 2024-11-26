using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenuController : MonoBehaviour
{
    public Transform menuPosition;           // Main menu position
    public Transform carSelectionPosition;   // Car selection position
    public float transitionDuration = 1.5f;  // Total time for the transition
    public MenuController menuController;

    private Transform targetPosition;        // Current target position for the camera
    private bool isTransitioning = false;
    private float transitionTime = 0f;
    private Vector3 startPosition;           // Starting position of the transition
    private Quaternion startRotation;        // Starting rotation of the transition

    void Start()
    {
        // Set the camera to the main menu position at the start
        transform.position = menuPosition.position;
        transform.rotation = menuPosition.rotation;
        targetPosition = menuPosition;
    }

    void Update()
    {
        if (isTransitioning && targetPosition != null)
        {
            // Increment transition time based on real time
            transitionTime += Time.deltaTime;

            // Calculate easing using SmoothStep for smooth easing
            float t = transitionTime / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate position and rotation using the eased value of t
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetPosition.rotation, t);

            // Stop transitioning when the duration is complete
            if (transitionTime >= transitionDuration)
            {
                isTransitioning = false;
                transitionTime = 0f;
            }
        }
    }

    public void OnPlayButton()
    {
        // Set target to car selection position and start transition
        targetPosition = carSelectionPosition;
        menuController.mainMenuPanel.gameObject.SetActive(false);
        Invoke("SetCarSelectionPanel", 1.5f);
        StartTransition();
    }

    public void OnBackButton()
    {
        // Set target to main menu position and start transition
        targetPosition = menuPosition;
        menuController.carSelectionPanel.gameObject.SetActive(false);
        Invoke("SetMainMenuPanel", 1.5f);
        StartTransition();
    }

    private void StartTransition()
    {
        isTransitioning = true;
        transitionTime = 0f;                  // Reset transition timer
        startPosition = transform.position;   // Set the starting position
        startRotation = transform.rotation;   // Set the starting rotation
    }

    private void SetMainMenuPanel()
    {
        menuController.mainMenuPanel.gameObject.SetActive(true);
    }

    private void SetCarSelectionPanel()
    {
        menuController.carSelectionPanel.gameObject.SetActive(true);
    }
}
