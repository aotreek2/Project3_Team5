using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private GameObject[] aiRacers;
    [SerializeField] private GameObject player, trapPanel;
    [SerializeField] private Camera trapCamera;
    [SerializeField] private GameObject playerCamera;
    private int countdown = 3;
    [SerializeField] private Collider finishLine;
    [SerializeField] private ResultManager results;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;

    void Start()
    {
        countdownTxt.enabled = false;

        foreach (GameObject ai in aiRacers)
        {
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = false;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        playerCamera.GetComponent<AudioListener>().enabled = false;
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
        trapCamera.GetComponent<AudioListener>().enabled = false;
        playerCamera.GetComponent<AudioListener>().enabled = true;
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
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = true;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerInput.enabled = true;
        playerScript.enabled = true;
    }
    private void CheckFinish()
    {
        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        if (finishLine.bounds.Intersects(player.GetComponent<Collider>().bounds))
        {
            results.FinishRace(playerScript.carName, "Player");
            results.AddRacers(playerScript.gameObject);
            SceneManager.LoadScene("RaceResults");
            return;
        }

        foreach (GameObject ai in aiRacers)
        {
            AICarController aiRacer = ai.GetComponent<AICarController>();
            if (finishLine.bounds.Intersects(aiRacer.GetComponentInChildren<Collider>().bounds))
            {
                results.FinishRace(aiRacer.aiName, "AI");
                results.AddRacers(aiRacer.gameObject);
                return;
            }
        }

        
    }
}
