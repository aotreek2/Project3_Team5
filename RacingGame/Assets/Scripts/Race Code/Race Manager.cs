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
    private int countdown = 3;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;

    void Start()
    {
        Cursor.visible = true;
        countdownTxt.enabled = false;

        foreach (GameObject ai in aiRacers)
        {
            AIRacer aiScript = ai.GetComponent<AIRacer>();
            aiScript.enabled = false;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerInput.enabled = false;
        playerScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpRace()
    {
        Cursor.visible = false;
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
}
