using System.Collections;
using UnityEngine;

public class BoundaryReset : MonoBehaviour
{
    public GameObject player; // Reference to the player's GameObject.
    private Vector3 entryPosition; // Position where the player entered the trigger.
    private Coroutine resetTimerCoroutine; // Coroutine for managing the reset timer.
    private PlayerSpawn playerCar; // Reference to the PlayerSpawn script.

    private void Start()
    {
        // Delayed initialization of the player GameObject.
        Debug.Log("BoundaryReset: Starting delayed initialization.");
        Invoke("DelayedStart", 0.15f);
    }

    private void DelayedStart()
    {
        // Find the PlayerSpawn script and assign the currentCar GameObject as the player.
        playerCar = FindObjectOfType<PlayerSpawn>();

        if (playerCar != null)
        {
            player = playerCar.currentCar;
            Debug.Log($"BoundaryReset: Player assigned as {player.name}");
        }
        else
        {
            Debug.LogError("BoundaryReset: PlayerSpawn script not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player.
        if (other.gameObject == player)
        {
            // Store the position where the player entered the trigger.
            entryPosition = player.transform.position;
            Debug.Log($"BoundaryReset: Player entered trigger zone at position {entryPosition}");

            // Start the reset timer if it's not already running.
            if (resetTimerCoroutine == null)
            {
                Debug.Log("BoundaryReset: Starting reset timer.");
                resetTimerCoroutine = StartCoroutine(ResetTimer());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player.
        if (other.gameObject == player)
        {
            Debug.Log("BoundaryReset: Player exited trigger zone.");

            // Cancel the reset timer if the player exits the trigger.
            if (resetTimerCoroutine != null)
            {
                Debug.Log("BoundaryReset: Canceling reset timer.");
                StopCoroutine(resetTimerCoroutine);
                resetTimerCoroutine = null;
            }
        }
    }

    private IEnumerator ResetTimer()
    {
        Debug.Log("BoundaryReset: Timer started.");

        // Wait for 3 seconds.
        yield return new WaitForSeconds(3f);

        Debug.Log($"BoundaryReset: Timer completed. Resetting player position to {entryPosition}");

        // Reset the player's position to where they entered the trigger.
        player.transform.position = entryPosition;

        // Reset the timer coroutine reference.
        resetTimerCoroutine = null;
    }
}
