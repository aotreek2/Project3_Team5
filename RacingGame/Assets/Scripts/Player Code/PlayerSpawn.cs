using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] GameObject sedanPrefab, coupePrefab, truckPrefab;
    private GameObject currentCar;
    // Start is called before the first frame update
    void Start()
    {
        string selectedCar = PlayerPrefs.GetString("SelectedCar", "Coupe");
        LoadCar(selectedCar);
    }

    void LoadCar(string carType)
    {
        // Destroy the previously loaded car if any
        if (currentCar != null)
        {
            Destroy(currentCar);
        }

        // Instantiate the car based on the selection
        switch (carType)
        {
            case "Coupe":
                currentCar = Instantiate(coupePrefab, transform.position, transform.rotation);
                break;
            case "Sedan":
                currentCar = Instantiate(sedanPrefab, transform.position, transform.rotation);
                break;
            case "Truck":
                currentCar = Instantiate(truckPrefab, transform.position, transform.rotation);
                break;
            default:
                Debug.LogError("Unknown car type selected: " + carType);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
