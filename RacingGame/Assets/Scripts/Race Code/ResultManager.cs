using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private List<string> raceResults = new List<string>();
    [SerializeField] public List<GameObject> racers, spawnPoints = new List<GameObject>();

    Scene currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        DisplayResults();
    }


    private void DisplayResults()
    {
        if (currentScene.name == "RaceResults")
        {
            for (int i = 0; i <= 3; i++)
            {
                
            }

            for (int i = 0; i < 3; i++)
            {
                Instantiate(racers[i]);
                racers[i].transform.position = spawnPoints[i].transform.position;
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
        racers.Add(racerModel);
    }

    public List<string> GetRaceResults()
    {
        return raceResults;
    }
}
