using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour, IGameState
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TMP_Text lobbyStatusText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button onlyAiButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject aiDifficultyPanel;
    [SerializeField] private Button btnBasePrefab;

    private bool isReadyToStart;
    private bool pressedReadyToStart;
    private GameModes _gameMode;
    private FusionManager.ConnectionStatus _status;


    private void Start()
    {
        lobbyStatusText.text = "Lobby";
        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(OnStartGameButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        onlyAiButton.onClick.AddListener(OnOnlyAiButtonClick);
    }
    #region IGameState
    public void EnterState()
    {
        lobbyPanel.SetActive(true);
        switch (GameModeManager.Instance.CurrentGameMode)
        {
            case GameModes.SingleDesktop:
                _gameMode = GameModes.SingleDesktop;
                SinglePlayerOptions();
                lobbyStatusText.text = "Select AI Level";
                break;
            case GameModes.MultiPlayerDesktop:
                _gameMode = GameModes.MultiPlayerDesktop;
                MultiPlayerOptions();
                lobbyStatusText.text = "Starting Connection";
                break;
        }
        startGameButton.interactable = false;
        backButton.interactable = true;
    }

    public void UpdateState()
    {
        if (isReadyToStart)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }

        if (_gameMode == GameModes.MultiPlayerDesktop)
        {
            //OnAllPlayersReady();
        }
    }

    public void ExitState()
    {
        EnableReadyToStart(false);
        onlyAiButton.gameObject.SetActive(false);
        aiDifficultyPanel.SetActive(false);
        backButton.interactable = false;
        lobbyPanel.SetActive(false);
    }
    #endregion IGameState

    #region Lobby Methods
    public void SetLobbyStatus(string status)
    {
        // Update the lobby status text
        lobbyStatusText.text = status;
    }
    public void StartGame()
    {
        // Load the "GameScene" asynchronously        
        GameStateManager.Instance.ChangeState(GameStates.GAMEPLAY);
    }
    private void OnStartGameButtonClick()
    {
        if (_gameMode == GameModes.SingleDesktop || _gameMode == GameModes.SingleOVR)
        {
            SpawnManager.Instance.SpawnLocalPlayer();
            StartGame();
        }
        else if (_gameMode == GameModes.OnlyAi)
        {
            SpawnManager.Instance.SpawnOnlyAi();
            StartGame();
        }
        else
        {
            OnPlayerReadyChange(true);
        }
    }
    private void OnBackButtonClick()
    {
        if (_gameMode == GameModes.MultiPlayerDesktop)
        {
            if (pressedReadyToStart)
                OnPlayerReadyChange(false);
            FusionManager.Instance.DisconectPlayer();
        }
        GameStateManager.Instance.ChangeState(GameStates.MAINMENU);
    }
    #endregion

    #region SinglePlayerOptions
    void SinglePlayerOptions()
    {
        onlyAiButton.gameObject.SetActive(true);
        aiDifficultyPanel.SetActive(true);
        if (aiDifficultyPanel.transform.childCount > 0)
        {
            return;
        }
        foreach (GameDifficulty gameDifficulty in GameDifficulty.GetValues(typeof(GameDifficulty)))
        {
            Button btn = Instantiate(btnBasePrefab, aiDifficultyPanel.transform);
            btn.GetComponentInChildren<TMP_Text>().text = gameDifficulty.ToString();
            btn.onClick.AddListener(() => OnAiLevelButtonClick(gameDifficulty));
        }
    }
    void OnOnlyAiButtonClick()
    {
        GameModeManager.Instance.SetGameMode(GameModes.OnlyAi);
        _gameMode = GameModes.OnlyAi;
        OnStartGameButtonClick();
    }
    void OnAiLevelButtonClick(GameDifficulty gameDifficulty)
    {
        GameModeManager.Instance.SetGameDifficulty(gameDifficulty);
        EnableReadyToStart(true);
    }
    #endregion SinglePlayerOptions

    #region MultiPlayerOptions
    void MultiPlayerOptions()
    {
        aiDifficultyPanel.SetActive(false);
        FusionManager.Instance.LaunchLobby(GameMode.Shared, "TestRoom", 2, OnConnectionStatusUpdate, OnMaxPlayersReached, OnAllPlayersReady);
        //FusionLauncher.Instance.Launch(GameMode.Shared, "TestRoom", 2, OnConnectionStatusUpdate, OnSpawnPlayer, OnDespawnPlayer)
    }
    private void OnMaxPlayersReached(bool isMaxPlayersReached)
    {
        EnableReadyToStart(isMaxPlayersReached);
    }

    public void EnableReadyToStart(bool ready)
    {
        // Update the ready state and trigger a state update
        isReadyToStart = ready;
        UpdateState();
    }
    private void OnPlayerReadyChange(bool ready)
    {
        pressedReadyToStart = ready;
        //ReadyCheck.Instance.Rpc_PlayerReadyChange(ready);
    }
    private void OnAllPlayersReady()
    {
        StartGame();
    }

    private void UpdateConnectionUI()
    {
        switch (_status)
        {
            case FusionManager.ConnectionStatus.Disconnected:
                lobbyStatusText.text = "Disconnected!";
                break;
            case FusionManager.ConnectionStatus.Failed:
                lobbyStatusText.text = "Failed!";
                break;
            case FusionManager.ConnectionStatus.Connecting:
                lobbyStatusText.text = "Connecting";
                break;
            case FusionManager.ConnectionStatus.Connected:
                lobbyStatusText.text = "Waiting for players...";
                break;
        }
    }
    private void OnConnectionStatusUpdate(NetworkRunner runner, FusionManager.ConnectionStatus status, string reason)
    {
        if (!this)
            return;

        Debug.Log(status);

        if (status != this._status)
        {
            switch (status)
            {
                case FusionManager.ConnectionStatus.Disconnected:
                    lobbyStatusText.text = reason;
                    break;
                case FusionManager.ConnectionStatus.Failed:
                    lobbyStatusText.text = reason;
                    break;
            }
        }

        this._status = status;
        UpdateConnectionUI();
    }
    #endregion MultiPlayerOptions


}
