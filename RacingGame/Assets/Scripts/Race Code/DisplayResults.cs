using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayResults : MonoBehaviour
{
    [SerializeField] public ResultManager results;
    [SerializeField] public RaceManager race;
    PlayerSpawn playerCar;
    private List<GameObject> racersList;
    [SerializeField] private Transform[] placementPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] racers;
    [SerializeField] private AudioSource music;
    private void Awake()
    {
        
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
                for (int i = 0; i < racersList.Count; i++)
                {
                    GameObject displayedCar = Instantiate(racersList[i]);
                    displayedCar.GetComponentInChildren<AudioSource>().Stop();
                    var scripts = displayedCar.GetComponents<MonoBehaviour>();
                    foreach (var script in scripts)
                    {
                        script.enabled = false;
                    }

                    displayedCar.transform.position = placementPoints[i].transform.position;
                    displayedCar.transform.rotation = placementPoints[i].transform.rotation;

                }
            }
        }
        music.Stop();
        race.winCam.enabled = true;
    }
}

