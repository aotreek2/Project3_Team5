using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayResults : MonoBehaviour
{
    [SerializeField] public ResultManager results;
    private List<GameObject> racersList;
    [SerializeField] private Transform[] placementPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] racers;

    private void Awake()
    {
        results = FindObjectOfType<ResultManager>();
    }

    void Start()
    {

    }

    public void Display()
    {
        racersList = results.racers;

        if (racersList != null)
        {
            foreach (var racer in racersList)
            {
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(racersList[i]);
                    racersList[i].transform.position = placementPoints[i].transform.position;
                    racersList[i].transform.rotation = placementPoints[i].transform.rotation;

                }
            }
        }
    }
}

