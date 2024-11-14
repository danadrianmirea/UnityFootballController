using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Text exitPromptText;  // Assign this in the Inspector with the Text UI element
    public GameObject panel;
    private bool isPaused = false;

    void Start()
    {
        // Ensure the exit prompt text is hidden initially
        exitPromptText.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    void Update()
    {
        // Check for Escape key press to toggle pause
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

        // Check for Y or N keys if the game is paused
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                QuitGame();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                ResumeGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Pauses the game
        exitPromptText.text = "Are you sure you want to exit? (Y/N)";
        exitPromptText.gameObject.SetActive(true);  // Show the exit prompt
        panel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Resumes the game
        exitPromptText.gameObject.SetActive(false);  // Hide the exit prompt
        panel.SetActive(false);
    }

    void QuitGame()
    {
        // In the editor, this will stop play mode, but in a build, it will quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
