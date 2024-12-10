using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlacementManager : MonoBehaviour
{
    public GameObject finishLine; // The finish line GameObject.
    public TMP_Text placementText; // Text field to display the player's placement.
    private PlayerSpawn playerCar; // Reference to the PlayerSpawn script.
    public GameObject player; // Reference to the player's GameObject.

    public List<GameObject> cars = new List<GameObject>(); // List of all cars in the scene.

    void Start()
    {
        Invoke("DelayedStart", 0.15f);
    }

    private void DelayedStart()
    {
        // Find the PlayerSpawn script and assign the currentCar GameObject as the player.
        playerCar = FindObjectOfType<PlayerSpawn>();

        if (playerCar != null)
        {
            player = playerCar.currentCar;
            Debug.Log($"PlacementManager: Player assigned as {player.name}");

            // Ensure the player is added to the car list.
            if (!cars.Contains(player))
            {
                cars.Add(player);
            }
        }
        else
        {
            Debug.LogError("PlacementManager: PlayerSpawn script not found in the scene!");
        }

        // Automatically find and add all AI cars to the cars list.
        foreach (var car in GameObject.FindGameObjectsWithTag("Car"))
        {
            if (!cars.Contains(car))
            {
                cars.Add(car);
            }
        }

        Debug.Log($"PlacementManager: Total cars in list: {cars.Count}");
    }

    void Update()
    {
        UpdatePlacement();
    }

    private void UpdatePlacement()
    {
        if (cars == null || cars.Count == 0 || finishLine == null)
        {
            Debug.LogError("PlacementManager: Cars list or finish line is not set up correctly!");
            return;
        }

        // Create a dictionary to store distances.
        Dictionary<GameObject, float> distances = new Dictionary<GameObject, float>();

        foreach (GameObject car in cars)
        {
            if (car != null)
            {
                // Calculate the distance from the car to the finish line.
                float distance = Vector3.Distance(car.transform.position, finishLine.transform.position);
                distances[car] = distance;
            }
        }

        // Sort cars by distance to the finish line.
        var sortedCars = new List<KeyValuePair<GameObject, float>>(distances);
        sortedCars.Sort((x, y) => x.Value.CompareTo(y.Value));

        // Find the player's placement.
        int playerPlacement = sortedCars.FindIndex(pair => pair.Key == player) + 1;

        if (playerPlacement > 0)
        {
            // Update the text field to display the player's placement.
            placementText.text = $"Placement: {playerPlacement}/{cars.Count}";
        }
        else
        {
            placementText.text = "Placement: N/A";
            Debug.LogWarning("PlacementManager: Player car not found in placement list!");
        }
    }
}
