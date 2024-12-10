using System.Collections;
using TMPro;
using UnityEngine;

public class BoundaryReset : MonoBehaviour
{
    public GameObject player; // Reference to the player's GameObject.
    public float heightThreshold = 23f; // The Y-value threshold for resetting.
    public float resetDelay = 3f; // Time before resetting the player.
    public TMP_Text boundaryText;
    private Vector3 entryPosition; // Position where the player first exceeded the threshold.
    private Quaternion entryRotation;
    private Coroutine resetTimerCoroutine; // Coroutine for managing the reset timer.
    private PlayerSpawn playerCar; // Reference to the PlayerSpawn script.
    private CarMoveScr playerScript;

    private void Start()
    {
        // Delayed initialization of the player GameObject.
        Invoke("DelayedStart", 0.15f);
    }

    private void DelayedStart()
    {
        // Find the PlayerSpawn script and assign the currentCar GameObject as the player.
        playerCar = FindObjectOfType<PlayerSpawn>();

        if (playerCar != null)
        {
            player = playerCar.currentCar;
            playerScript = player.GetComponent<CarMoveScr>();
            Debug.Log($"BoundaryReset: Player assigned as {player.name}");
        }
        else
        {
            Debug.LogError("BoundaryReset: PlayerSpawn script not found in the scene!");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Check if the player's Y position exceeds the threshold.
            if (player.transform.position.y > heightThreshold)
            {
                // If the timer isn't running, start it and store the entry position.
                if (resetTimerCoroutine == null)
                {
                    entryPosition = player.transform.position;
                    entryRotation = player.transform.rotation; // Reset rotation to default (no rotation)
                    boundaryText.enabled = true;
                    boundaryText.text = "Get back to the track";
                    Debug.Log($"BoundaryReset: Player exceeded height threshold at position {entryPosition}");
                    resetTimerCoroutine = StartCoroutine(ResetTimer());
                }
            }
            else
            {
                // If the player goes below the threshold, stop the timer.
                if (resetTimerCoroutine != null)
                {
                    Debug.Log("BoundaryReset: Player returned below threshold. Canceling reset timer.");
                    boundaryText.enabled = false;
                    StopCoroutine(resetTimerCoroutine);
                    resetTimerCoroutine = null;
                }
            }
        }
    }

    private IEnumerator ResetTimer()
    {
        Debug.Log("BoundaryReset: Timer started.");

        float timer = resetDelay;

        // Update the text to show the countdown until reset.
        while (timer > 0)
        {
            boundaryText.text = $"Get back to the track: {timer:F1} seconds remaining";
            timer -= Time.deltaTime;
            yield return null; // Wait for the next frame.
        }

        // When the timer completes, reset the player's position and rotation.
        Debug.Log($"BoundaryReset: Timer completed. Resetting player position to {entryPosition}");
        player.transform.position = entryPosition;
        player.transform.rotation = entryRotation;

        // Call the reset method on the player's script if necessary.
        playerScript.BoundaryReset();

        // Hide the text after the reset.
        boundaryText.enabled = false;

        // Reset the timer coroutine reference.
        resetTimerCoroutine = null;
    }
}

