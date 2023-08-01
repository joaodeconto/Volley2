using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private IGameState _currentState;

    public static GameStateManager Instance;

    public GameStates CurrentState { get; private set; }
    public GameStates PreviousState { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // Set the initial game state
        ChangeState(GameStates.MAINMENU);
    }

    public void ChangeState(GameStates newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        if (_currentState != null)
        {
            _currentState.ExitState();
        }

        PreviousState = CurrentState;
        CurrentState = newState;

        switch (newState)
        {
            case GameStates.MAINMENU:
                _currentState = FindObjectOfType<MainMenu>();
                //Debug.Log("Entered Main Menu State");
                break;
            case GameStates.LOBBY:
                _currentState = FindObjectOfType<LobbyManager>();
                //Debug.Log("Entered Lobby State");
                break;
            case GameStates.BREAK:
                //Debug.Log("Entered Break State");
                _currentState = FindObjectOfType<BreakManager>();
                break;
            case GameStates.GAMEPLAY:
                _currentState = FindObjectOfType<GameManager>();
                //Debug.Log("Entered Gameplay State");
                break;
            case GameStates.PAUSED:
                break;
            case GameStates.GAMEOVER:
                _currentState = FindObjectOfType<GameOver>();
                //Debug.Log("Entered Game Over State");
                break;
        }
        if (_currentState != null)
        {
            _currentState.EnterState();
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.UpdateState();
        }
    }
}
