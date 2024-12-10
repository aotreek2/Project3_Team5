using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private List<string> raceResults = new List<string>();
    [SerializeField] public List<GameObject> racers = new List<GameObject>();
    public static ResultManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FinishRace(string racerName, string racerType)
    {
        string result = $"{racerName} ({racerType})";
        if (!raceResults.Contains(result))
        {
            raceResults.Add(result);
            ResultManager.Instance.raceResults = raceResults;
            Debug.Log($"{racerName} ({racerType}) finished at position {raceResults.Count}");
        }
    }

    public void AddRacers(GameObject racerModel)
    {
        racers.Add(racerModel);
        foreach (var racer in racers)
        {
            Debug.Log(racer);
        }
        ResultManager.Instance.racers = racers;
    }

    public List<string> GetRaceResults()
    {
        return raceResults;
    }
}
