using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameTrapManager : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject[] traps; // List of available traps.
    [SerializeField] private Transform trapPlacementPoint; // Point where traps will be placed (attached to the player car).
    [SerializeField] private int maxTraps = 5; // Maximum number of traps that can be placed.

    private int trapIndex = 0; // Current trap index.
    private int trapsPlaced = 0; // Number of traps placed.
    private GameObject trapPreview; // The currently previewed trap.

    private Camera playerCamera;
    private TMPro.TMP_Text trapsLeft; // Dynamically retrieved traps left text.
    private TMPro.TMP_Text currentTrap; // Dynamically retrieved current trap text.

    void Start()
    {
        playerCamera = Camera.main; // Get the player's camera.

        // Find text fields dynamically in the scene.
        GameObject trapsLeftGO = GameObject.Find("TrapsLeft");
        GameObject currentTrapGO = GameObject.Find("Current Trap");

        if (trapsLeftGO != null)
        {
            trapsLeft = trapsLeftGO.GetComponent<TMPro.TMP_Text>();
        }
        else
        {
            Debug.LogError("InGameTrapManager: Could not find TrapsLeft text field!");
        }

        if (currentTrapGO != null)
        {
            currentTrap = currentTrapGO.GetComponent<TMPro.TMP_Text>();
        }
        else
        {
            Debug.LogError("InGameTrapManager: Could not find CurrentTrap text field!");
        }

        // Update the traps left UI text.
        if (trapsLeft != null)
        {
            trapsLeft.text = "Traps Left: " + (maxTraps - trapsPlaced);
        }

        PreviewTrap();
    }

    void Update()
    {
        PreviewTrapPosition();
        PlaceTrap();
        SwitchTraps();
    }

    private void PreviewTrap()
    {
        if (trapPreview != null)
        {
            Destroy(trapPreview); // Destroy existing preview.
        }

        trapPreview = Instantiate(traps[trapIndex], trapPlacementPoint.position, trapPlacementPoint.rotation);
        trapPreview.GetComponent<Collider>().enabled = false; // Disable collision for the preview.

        // Update the current trap UI text.
        if (currentTrap != null)
        {
            currentTrap.text = "Trap: " + traps[trapIndex].name;
        }
    }

    private void PreviewTrapPosition()
    {
        if (trapPreview != null)
        {
            // Keep the preview at the placement point.
            trapPreview.transform.position = trapPlacementPoint.position;
            trapPreview.transform.rotation = trapPlacementPoint.rotation;
        }
    }

    private void PlaceTrap()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Spacebar pressed.
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // Ignore if clicking on UI.

            if (trapsPlaced < maxTraps)
            {
                // Place the trap at the placement point.
                Instantiate(traps[trapIndex], trapPlacementPoint.position, trapPlacementPoint.rotation);
                trapsPlaced++;

                // Update traps left UI text.
                if (trapsLeft != null)
                {
                    trapsLeft.text = "Traps Left: " + (maxTraps - trapsPlaced);
                }

                if (trapsPlaced == maxTraps && trapPreview != null)
                {
                    trapPreview.SetActive(false); // Disable preview after max traps placed.
                }
            }
        }
    }

    private void SwitchTraps()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            trapIndex = (trapIndex - 1 + traps.Length) % traps.Length; // Cycle backward.
            PreviewTrap();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            trapIndex = (trapIndex + 1) % traps.Length; // Cycle forward.
            PreviewTrap();
        }
    }
}


