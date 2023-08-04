using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour, IScore, IGameState
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int targetScore = 7;
    [SerializeField] private GameObject scenarioObj;

    public int MyTeam;
    public int LastPoint { get { return lastPoint; } }
    private int scoreTeamA = 0;
    private int scoreTeamB = 0;
    private int lastPoint = 0;



    public static UnityAction OnClearScore;
    public static UnityAction OnStateEnter;
    public static UnityAction OnStateUpdate;
    public static UnityAction OnStateExit;

    void Awake()
    {
        Instance = this;
        //scenarioObj.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        BallController.OnBallHitCourt += AddPoint;
    }
    private void OnDisable()
    {
        BallController.OnBallHitCourt -= AddPoint;
    }

    private bool CheckWin()
    {
        if (scoreTeamA == targetScore || scoreTeamB == targetScore)
            return true;
        else
            return false;
    }
    private void StartGame()
    {
        scenarioObj.SetActive(true);
        ClearScore();
    }

    public void RestartMatch()
    {
        ClearScore();
    }

    private void EndGame()
    {
        Result(scoreTeamA == targetScore ? 0 : 1);
        GameStateManager.Instance.ChangeState(GameStates.GAMEOVER);
    }

    #region IScore
    public void AddPoint(int team)
    {
        lastPoint = team;
        if (team == 0)
            scoreTeamA++;
        else
            scoreTeamB++;

        if (CheckWin())
            EndGame();
        else
            GameStateManager.Instance.ChangeState(GameStates.BREAK);
    }

    public void Result(int team)
    {

    }
    public void ClearScore()
    {
        scoreTeamB = scoreTeamA = 0;
        OnClearScore();
    }

    #endregion IScore

    #region IGameState
    public void EnterState()
    {
        if (GameStateManager.Instance.PreviousState == GameStates.LOBBY ||
            GameStateManager.Instance.PreviousState == GameStates.GAMEOVER)
        {
            StartGame();
        }
        OnStateEnter();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        Debug.Log("Exit GamePlay State");
        OnStateExit();
    }

    #endregion IGameState
}
