using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour, IScore, IGameState
{
    public static GameManager Instance { get; private set; }
    public static UnityAction OnClearScore;

    //[SerializeField] private float gravity = -6f;
    [SerializeField] private int targetScore = 7;
    [SerializeField] private GameObject scenarioObj;

    public int MyTeam;
    public GameObject ballObj;
    private Vector3 ballRandomPosition;
    private int scoreTeamA = 0;
    private int scoreTeamB = 0;

    public static UnityAction OnStateEnter;
    public static UnityAction OnStateUpdate;
    public static UnityAction OnStateExit;

    void Awake()
    {
        Instance = this;
        //Physics.gravity = new Vector3(0, gravity, 0);
        //don't destroy this object when load new scene
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
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void StartGame()
    {
        scenarioObj.SetActive(true);
        ClearScore();
        HasBall();
        RestartBall();
    }

    //make function to end game
    private void EndGame()
    {
        Result(scoreTeamA == targetScore ? 0 : 1);      
        GameStateManager.Instance.ChangeState(GameStates.GAMEOVER);
    }

    public void RestartBall()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            NetworkObject b = ballObj.GetComponent<NetworkObject>();
            if (!b.HasStateAuthority)                
                return;
        }
        ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballRandomPosition = new Vector3(Random.Range(-10f, 10f), 7, Random.Range(-5f, 5f));
        ballObj.transform.position = ballRandomPosition;
}
    private GameObject HasBall()
    {
        if (ballObj != null)
            return ballObj;

        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            BallNetwork networkBall = FindObjectOfType<BallNetwork>();
            if (networkBall != null)
            {
                ballObj = networkBall.gameObject;
                Debug.Log("HasBall: " + ballObj);
                return ballObj;
            }
            else
            {
                // If the ball doesn't exist in the scene, spawn a new one and return it
                ballObj = SpawnManager.Instance.SpawnNetworkBall();
                return ballObj;
            }
        }
        else
        {
            if (ballObj == null)
            {
                ballObj = SpawnManager.Instance.SpawnLocalBall();
            }
            return ballObj;
        }
    }
    #region IScore
    public void AddPoint(int team)
    {
        if (team == 0)
            scoreTeamA++;
        else
            scoreTeamB++;

        if (CheckWin())
            EndGame();
        else
            RestartBall();
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
        
        if(GameStateManager.Instance.PreviousState == GameStates.LOBBY ||
            GameStateManager.Instance.PreviousState == GameStates.GAMEOVER)
        {
            StartGame();
        }
        else if(GameStateManager.Instance.PreviousState == GameStates.BREAK)
        {
            RestartBall();
        }
        OnStateEnter();
    }

    public void UpdateState()
    {
       
    }

    public void ExitState()
    {
        OnStateExit();
    }

    #endregion IGameState
}
