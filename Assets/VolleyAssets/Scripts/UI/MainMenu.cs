using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IGameState
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button quitButton;

    private void Init()
    {
        //if find a runner, shut it down
        if (TryGetComponent(out NetworkRunner networkRunner))
        {
            networkRunner.Shutdown();
        }
        // Add button click listeners
        singlePlayerButton.onClick.AddListener(OnSinglePlayerButtonClick);
        multiplayerButton.onClick.AddListener(OnMultiplayerButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    public void EnterState()
    {
        
        mainMenuPanel.SetActive(true);
        Init();
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
        Debug.Log("Exited Main Menu State");
        // Disable the main menu panel
        mainMenuPanel.SetActive(false);
    }

    private void OnSinglePlayerButtonClick()
    {
        if(GameModeManager.Instance.CurrentPlatformType == PlatformType.VR)
        {
            GameModeManager.Instance.SetGameMode(GameModes.SingleOVR);
        }
        else
        {
            GameModeManager.Instance.SetGameMode(GameModes.SingleDesktop);
        }
        GameStateManager.Instance.ChangeState(GameStates.LOBBY);
    }

    private void OnMultiplayerButtonClick()
    {
        if (GameModeManager.Instance.CurrentPlatformType == PlatformType.VR)
        {
            GameModeManager.Instance.SetGameMode(GameModes.MultiplayerOVR);
        }
        GameModeManager.Instance.SetGameMode(GameModes.MultiPlayerDesktop);
        GameStateManager.Instance.ChangeState(GameStates.LOBBY);
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
