using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private GameObject[] aiRacers;
    [SerializeField] private GameObject player, trapPanel;
    [SerializeField] private Camera trapCamera;
    [SerializeField] private GameObject playerCamera;
    private int countdown = 3;
    [SerializeField] private List<string> raceResults = new List<string>();
    [SerializeField] private Collider finishLine;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;

    void Start()
    {
        countdownTxt.enabled = false;

        foreach (GameObject ai in aiRacers)
        {
            AIRacer aiScript = ai.GetComponent<AIRacer>();
            aiScript.enabled = false;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        playerInput.enabled = false;
        playerScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFinish();
    }

    public void SetUpRace()
    {
        playerCamera.GetComponent<CarCameraController>().ConfineCursor();
        trapCamera.enabled = false;
        trapPanel.SetActive(false);
        countdownTxt.enabled = true;
        StartCountdown();
    }

    private void StartCountdown()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (countdown > 0)
        {
            countdownTxt.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Final message (optional)
        countdownTxt.text = "Go!";
        BeginRace();
        yield return new WaitForSeconds(1f);
        countdownTxt.text = ""; // Clear the text after the countdown ends
    }

    public void BeginRace()
    {
        foreach (GameObject ai in aiRacers)
        {
            AIRacer aiScript = ai.GetComponent<AIRacer>();
            aiScript.enabled = true;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerInput.enabled = true;
        playerScript.enabled = true;
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

    public List<string> GetRaceResults()
    {
        return raceResults;
    }

    private void CheckFinish()
    {
        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        if (finishLine.bounds.Intersects(player.GetComponent<Collider>().bounds))
        {
            FinishRace(playerScript.carName, "Player");
            return;
        }

        foreach (GameObject ai in aiRacers)
        {
            AIRacer aiRacer = ai.GetComponent<AIRacer>();
            if (finishLine.bounds.Intersects(aiRacer.GetComponent<Collider>().bounds))
            {
                FinishRace(aiRacer.aiName, "AI");
                return;
            }
        }

        
    }
}
