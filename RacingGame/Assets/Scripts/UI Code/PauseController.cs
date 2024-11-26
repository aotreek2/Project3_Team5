using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }              
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game (works only in a build)
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure the game resumes if going back to the main menu
        SceneManager.LoadScene("Main_Menu"); // Load your main menu scene
    }

    public void LevelOneLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }

    public void LevelTwoLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 2");
    }

    public void LevelThreeLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 3");
    }
}

