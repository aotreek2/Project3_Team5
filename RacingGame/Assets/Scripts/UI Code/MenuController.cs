using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel, creditsPanel, leftButton, rightButton;
    [SerializeField] private TMP_Text helpTitle, helpText;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(scene.buildIndex + 1);
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
