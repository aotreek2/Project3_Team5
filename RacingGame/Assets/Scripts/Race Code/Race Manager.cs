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
    [SerializeField] private Camera trapCamera;
    public GameObject playerCamera;
    private int countdown = 3;
    [SerializeField] private Collider finishLine;
    [SerializeField] private ResultManager results;
    [SerializeField] public Camera winCam;
    PlayerSpawn playerCar;
    public string playerName;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownTxt;
    [SerializeField] public TMP_Text inGameTrapText;
    public string levelToLoad;

    [Header("SFX")]
    [SerializeField] AudioSource levelMusic;
    [SerializeField] AudioSource countdownBeep;
    [SerializeField] AudioSource countdownGo;
    void Start()
    {
        results = results.GetComponent<ResultManager>();
        Invoke("DelayedStart", .1f);
        inGameTrapText.enabled = false;

        foreach (GameObject ai in aiRacers)
        {
            AICarController aiScript = ai.GetComponent<AICarController>();
            aiScript.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
   
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
            playerCamera.SetActive(false);
            playerCamera.GetComponent<AudioListener>().enabled = false;
            speedometerPanel.SetActive(false);
            winCam.enabled = false;
            playerScript.start = false;
            //playerInput.enabled = false;
            // playerScript.enabled = false;
        }
    }

    public void SetUpRace()
    {
        GameObject.Find("Virtual Camera").GetComponent<CarCamHandler>().ConfineCursor();
        trapCamera.enabled = false;
        trapCamera.GetComponent<TrapCameraController>().enabled = false;
        trapCamera.GetComponent<AudioListener>().enabled = false;
        playerCamera.GetComponent<AudioListener>().enabled = true;
        playerCamera.SetActive(true);
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
            countdownBeep.Play();
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
        playerScript.start = true;

        countdownGo.Play();
        levelMusic.Play();
        inGameTrapText.enabled = true;
    }
    public void CheckFinish(string racerName, string racerType, GameObject model)
    {
        results.FinishRace(racerName, racerType);
        results.AddRacers(model);
    }
}
