using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private List<string> raceResults = new List<string>();
    [SerializeField] public GameObject[] racers, spawnPoints;

    Scene currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        for (int i = 0; i <= 3; i++)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("Placements");
            spawnPoints.Append(spawn);

        }

        if (currentScene.name == "RaceResults")
        {
            for (int i = 0; i < racers.Count(); i++)
            {
                foreach (GameObject racer in racers)
                {
                    racer.transform.position = spawnPoints[i].transform.position;
                }
            }
        }
    }

    public void FinishRace(string racerName, string racerType)
    {
        string result = $"{racerName} ({racerType})";
        if (!raceResults.Contains(result))
        {
            raceResults.Add(result);
            Debug.Log($"{racerName} ({racerType}) finished at position {raceResults.Count}");
        }
    }

    public void AddRacers(GameObject racerModel)
    {
        racers.Append(racerModel);
    }

    public List<string> GetRaceResults()
    {
        return raceResults;
    }
}
