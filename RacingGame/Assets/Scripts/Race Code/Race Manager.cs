using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cinemachine;
public class RaceManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private GameObject[] aiRacers;
    [SerializeField] public GameObject player, trapPanel;
    [SerializeField] GameObject speedometerPanel;
    [SerializeField] private CinemachineFreeLook trapCamera;
    private GameObject playerCamera;
    private int countdown = 3;
    [SerializeField] private Collider finishLine;
    [SerializeField] private ResultManager results;
    PlayerSpawn playerCar;
    public string playerName;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;
    public string levelToLoad;

    void Start()
    {
        countdownTxt.enabled = false;
        Invoke("DelayedStart", .1f);

        foreach (GameObject ai in aiRacers)
        {
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("CheckFinish", 5f);
   
    }

    private void DelayedStart()
    {
        
        playerCar = FindObjectOfType<PlayerSpawn>();
        player = playerCar.currentCar;
        playerName = PlayerPrefs.GetString("Player Name");

        if (player != null)
        {
            CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
            InputManager playerInput = player.GetComponent<InputManager>();
            playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
            playerCamera.GetComponent<AudioListener>().enabled = false;
            speedometerPanel.SetActive(false);
            playerInput.enabled = false;
            playerScript.enabled = false;
        }
    }

    public void SetUpRace()
    {
        GameObject.Find("Virtual Camera").GetComponent<CarCamHandler>().ConfineCursor();
        trapCamera.enabled = false;
        trapCamera.GetComponent<TrapCameraController>().enabled = false;
        trapCamera.GetComponent<AudioListener>().enabled = false;
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

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerInput.enabled = true;
        playerScript.enabled = true;
    }
    public void CheckFinish(string racerName, string racerType, GameObject model)
    {
        results.FinishRace(racerName, racerType);
        results.AddRacers(model);
        SceneManager.LoadScene("RaceResults");
    }
}
