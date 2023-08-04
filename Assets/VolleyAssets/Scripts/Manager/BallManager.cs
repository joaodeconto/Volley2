using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public GameObject BallObject { get; private set; }
    private int lastPoint = 0;
    private Vector3 ballServicePosition;
    public static BallManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void OnEnable()
    {
        // Register event listeners when the script is enabled
        GameManager.OnStateEnter += RestartBall;
    }

    private void OnDisable()
    {
        // Unregister event listeners when the script is disabled
        GameManager.OnStateEnter -= RestartBall;
    }

    public async void RestartBall()
    {
        if (GameStateManager.Instance.PreviousState == GameStates.PAUSED)
            return;

        await GetBall();
        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            NetworkObject b = BallObject.GetComponent<NetworkObject>();
            if (!b.HasStateAuthority)
                return;
        }
        BallObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        lastPoint = GameManager.Instance.LastPoint;
        ballServicePosition = new Vector3(10 * (lastPoint == 0 ? 1 : -1), 3, 0);
        //Debug.Log("BallManager.RestartBall() ballServicePosition: " + ballServicePosition);
        BallObject.transform.position = ballServicePosition;
        BallObject.GetComponent<BallController>().ServeBall(lastPoint);
    }

    public async Task<GameObject> GetBall()
    {
        if (BallObject != null)
            return BallObject;

        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            BallNetwork networkBall = FindObjectOfType<BallNetwork>();
            if (networkBall != null)
            {
                BallObject = networkBall.gameObject;
                return BallObject;
            }
            else
            {
                BallObject = await SpawnManager.Instance.SpawnNetworkBall();
                return BallObject;
            }
        }
        else
        {
            if (BallObject == null)
            {
                BallObject = await SpawnManager.Instance.SpawnLocalBall();
            }
            return BallObject;
        }
    }
}
