using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class MenuController : MonoBehaviour
{
    public GameObject helpPanel, creditsPanel, mainMenuPanel, carSelectionPanel, leftButton, rightButton;
    [SerializeField] private TMP_Text helpTitle, helpText;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button sedanSelect, coupeSelect, truckSelect, startGameButton;

    [Header("SFX")]
    [SerializeField] AudioSource buttonHover;
    [SerializeField] AudioSource buttonClicked;
    private Scene scene;
    void Start()
    {
        scene = SceneManager.GetActiveScene();
        startGameButton.interactable = false;

        if(scene.name == "Main_Menu")
        {
            mainMenuPanel.gameObject.SetActive(true);
            carSelectionPanel.gameObject.SetActive(false);
            helpPanel.gameObject.SetActive(false);
            creditsPanel.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DisableStart();
    }
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(scene.buildIndex + 1);
        buttonClicked.Play();
    }
    public void OnHelpButtonClicked()
    {
        helpPanel.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);
        buttonClicked.Play();
    }
    public void OnCreditsButtonClicked()
    {
        creditsPanel.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);
        buttonClicked.Play();
    }
    public void OnBackButtonClicked()
    {
        helpPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(false);
        mainMenuPanel.gameObject.SetActive(true);
        buttonClicked.Play();
    }

    public void OnRightButtonClicked()
    {
        helpTitle.text = "Controls";
        helpText.text = "ASDW - Move <br> Shift/Space - Break, Move Faster (Trap Camera) <br> P/Escape " +
        "- Pause <br> Rotate  (Trap Camera)  - Q & E <br> Zoom in (Trap Camera) - Mouse Scroll Wheel <br> Place Trap (Trap Camera) - LMB";
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
        buttonClicked.Play();
    }

    public void OnLeftButtonClicked()
    {
        helpTitle.text = "Objective";
        helpText.text = "Before the race starts, you will place traps in order to slow down your opponents  on the track, first to finish wins!";
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);
        buttonClicked.Play();

    }

    public void InputName()
    {
        string playerName = nameInput.text;

        PlayerPrefs.SetString("Player Name", playerName);
        PlayerPrefs.Save();
    }

    public void OnSedanSelect()
    {
        sedanSelect.interactable = false;
        coupeSelect.interactable = true;
        truckSelect.interactable = true;
        startGameButton.interactable = true;
        DisableStart();
        PlayerPrefs.SetString("SelectedCar", "Sedan");
        PlayerPrefs.Save();
        buttonClicked.Play();
    }
    public void OnCoupeSelect()
    {
        sedanSelect.interactable = true;
        coupeSelect.interactable = false;
        truckSelect.interactable = true;
        startGameButton.interactable = true;
        DisableStart();
        PlayerPrefs.SetString("SelectedCar", "Coupe");
        PlayerPrefs.Save();
        buttonClicked.Play();
    }
    public void OnTruckSelect()
    {
        sedanSelect.interactable = true;
        coupeSelect.interactable = true;
        truckSelect.interactable = false;
        startGameButton.interactable = true;
        DisableStart();
        PlayerPrefs.SetString("SelectedCar", "Truck");
        PlayerPrefs.Save();
        buttonClicked.Play();
    }

    private void DisableStart()
    {
        if(string.IsNullOrEmpty(nameInput.text))
        {
            startGameButton.interactable = false;
        }
        else
        {
            startGameButton.interactable = true;
        }
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnButtonHover()
    {
        buttonHover.Play();
    }
}
