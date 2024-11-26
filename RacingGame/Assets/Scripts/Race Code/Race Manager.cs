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
    [SerializeField] GameObject speedometerPanel;
    [SerializeField] private Camera trapCamera;
    private GameObject playerCamera;
    private int countdown = 3;
    [SerializeField] private Collider finishLine;
    [SerializeField] private ResultManager results;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;
    public string levelToLoad;

    void Start()
    {
       // countdownTxt.enabled = false;

        foreach (GameObject ai in aiRacers)
        {
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = false;
        }
        Invoke("DelayedStart", .5f);
    }

    // Update is called once per frame
    void Update()
    {
        CheckFinish();
        /*float distance = Vector3.Distance(player.transform.position, finishLine.transform.position);
        if(distance <= 30)
        {
            SceneManager.LoadScene(levelToLoad);
        }*/
    }

    private void DelayedStart()
    {
        /*
        player = GameObject.FindGameObjectWithTag("Player");
        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        playerCamera.GetComponent<AudioListener>().enabled = false;
        speedometerPanel.SetActive(false);
        playerInput.enabled = false;
        playerScript.enabled = false; */
    }

    public void SetUpRace()
    {
        GameObject.Find("Virtual Camera").GetComponent<CarCamHandler>().ConfineCursor();
        trapCamera.enabled = false;
        trapCamera.GetComponent<TrapCameraController>().enabled = false;
        trapCamera.GetComponent<AudioListener>().enabled = false;
       // playerCamera.GetComponent<AudioListener>().enabled = true;
        trapPanel.SetActive(false);
        //speedometerPanel.SetActive(true);
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

        countdownTxt.text = "Go!";
        BeginRace();
        yield return new WaitForSeconds(1f);
        countdownTxt.text = ""; 
    }

    public void BeginRace()
    {
        foreach (GameObject ai in aiRacers)
        {
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = true;
        }

        //CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
       // InputManager playerInput = player.GetComponent<InputManager>();
      //  playerInput.enabled = true;
       // playerScript.enabled = true;
    }
    private void CheckFinish()
    {
       /* CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        if (finishLine.bounds.Intersects(player.GetComponent<Collider>().bounds))
        {
            results.FinishRace(playerScript.carName, "Player");
            results.AddRacers(playerScript.gameObject);
            SceneManager.LoadScene("RaceResults");
            return;
        } */

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
