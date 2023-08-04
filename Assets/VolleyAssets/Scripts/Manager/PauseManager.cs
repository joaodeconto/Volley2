using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour, IGameState
{
    private bool isGamePaused = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;


    private void Start()
    {
        // Add listeners to the buttons
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }
    public void EnterState()
    {
        // Show the pause menu and pause the game
        Time.timeScale = 0f;
        isGamePaused = true;
        pauseMenu.SetActive(true);
    }

    public void ExitState()
    {
        // Hide the pause menu and resume the game
        Time.timeScale = 1f;
        isGamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void UpdateState()
    {
    }

    public void ResumeGame()
    {
        // Resume the game by changing the state to GAMEPLAY
        GameStateManager.Instance.ChangeState(GameStateManager.Instance.PreviousState);
    }

    public void RestartGame()
    {
        // Restart the game by reloading the current scene
        GameStateManager.Instance.ChangeState(GameStates.GAMEPLAY);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}



