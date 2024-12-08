using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayResults : MonoBehaviour
{
    [SerializeField] private ResultManager results;

    [SerializeField] private Transform[] placementPoints;
    void Start()
    {
        results = FindObjectOfType<ResultManager>();
        Display();
    }

    private void Display()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(results.racers[i]);
            results.racers[i].transform.position = placementPoints[i].transform.position;
            results.racers[i].transform.rotation = placementPoints[i].transform.rotation;

        }
    }
}

