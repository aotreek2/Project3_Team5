using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel, creditsPanel;
    private Scene scene;
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        if(scene.name == "Main_Menu")
        {
            helpPanel.gameObject.SetActive(false);
            creditsPanel.gameObject.SetActive(false);
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
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
