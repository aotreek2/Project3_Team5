using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel, creditsPanel, leftButton, rightButton;
    [SerializeField] private TMP_Text helpTitle, helpText;
    public CinemachineDollyCart dollyCart;
    public CinemachinePath initialPath;  // Side view path
    public CinemachinePath mainPath;     // Track path after selecting a car
    public float speed = 5f;

    private bool movingForward = false;
    private bool movingBackward = false;
    private Scene scene;
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if(scene.name == "Main_Menu")
        {
            helpPanel.gameObject.SetActive(false);
            creditsPanel.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
        }

        // Set initial path to the side view
        dollyCart.m_Path = initialPath;
        dollyCart.m_Position = 0f;
        dollyCart.m_Speed = 0f;  // Start with speed 0 to keep it stationary
    }

    // Update is called once per frame
    void Update()
    {
        // Stop the camera when it reaches the end of the path (either forward or backward)
        if (movingForward && dollyCart.m_Position >= dollyCart.m_Path.PathLength)
        {
            dollyCart.m_Speed = 0f;
            movingForward = false;
        }
        else if (movingBackward && dollyCart.m_Position <= 0f)
        {
            dollyCart.m_Speed = 0f;
            movingBackward = false;
        }
    }
    public void OnPlayButtonClicked()
    {
        dollyCart.m_Path = mainPath;
        movingForward = true;
        movingBackward = false;
        dollyCart.m_Speed = speed;  // Set speed to move forward
       // SceneManager.LoadScene(scene.buildIndex + 1);
    }
    public void OnBackToMainMenu()
    {
        dollyCart.m_Path = initialPath;
        movingForward = false;
        movingBackward = true;
        dollyCart.m_Speed = -speed;  // Set speed to move backward
    }
    public void OnHelpButtonClicked()
    {
        helpPanel.gameObject.SetActive(true);
    }
    public void OnCreditsButtonClicked()
    {
        creditsPanel.gameObject.SetActive(true);

    }
    public void OnBackButtonClicked()
    {
        helpPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(false);
    }

    public void OnRightButtonClicked()
    {
        helpTitle.text = "Controls";
        helpText.text = "ASDW - Move <br> Shift/Space - Break <br> P/Escape - Pause";
        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
    }

    public void OnLeftButtonClicked()
    {
        helpTitle.text = "Objective";
        helpText.text = "Before the race starts, you will place traps in order to slow down your opponents  on the track, first to finish wins!";
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);

    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
